using System;
using System.IO;
using System.Collections;
using Tao.OpenGl;


namespace Visualization
{
	/// <summary>
	/// Summary description for Parser.
	/// </summary>
	public class Parser
	{
		private	struct VariableStorer
		{
			public enum VarType
			{COORDINATE, DATA}
			public enum Var{ X = 0, Y = 1, Z = 2 }
			public VarType Type;
			public uint	Location;
			public VariableStorer(VarType v, uint loc) 
			{
				Type = v;
				Location = loc;
			}
			public void Store(ref double value, Vertex v)
			{
				if(Type == VarType.COORDINATE)
				{
					switch(Location)
					{
						case (uint)Var.X:
							v.Position.x = value;
							break;
						case (uint)Var.Y:
							v.Position.y = value;
							break;
						case (uint)Var.Z:
							v.Position.z = value;
							break;
							//default: error
					}
				}
				else //Type == DATA
				{
					v.Data[Location] = value;
				}
			}
		}
		void Advance()
		{
			currentToken = scanner.Tokenize(); 
		}
		void AddVariable()
		{
			VariableStorer vs = new VariableStorer();
			if(! Scanner.IsID( currentToken ) )
				throw new Exception("Parser: Error while parsing VARIABLES section.");
			if( scanner.Text == "x" || scanner.Text == "X" )
			{
				vs.Type = VariableStorer.VarType.COORDINATE;
				vs.Location = (uint)VariableStorer.Var.X;
			}
			else if( scanner.Text == "y" || scanner.Text == "Y" )
			{
				vs.Type = VariableStorer.VarType.COORDINATE;
				vs.Location = (uint)VariableStorer.Var.Y;
			}
			else if( scanner.Text == "z" || scanner.Text == "Z" )
			{
				vs.Type = VariableStorer.VarType.COORDINATE;
				vs.Location = (uint)VariableStorer.Var.Z;
			}
			else
			{
				//Note: there can be a fatal error if
				//the user specifies the same variable more than once.
				//Exactly, there will be one redundant location in the middle
				//of Data array of every Vertex.
				vs.Type = VariableStorer.VarType.DATA;
				vs.Location = cData++;
				mesh.VarToIndex[scanner.Text] = vs.Location;
			}
			storerTable.Add( vs );
			Advance();
		}
		bool MatchAndAdvance(TokenType token)
		{
			if(token == currentToken)
			{
				Advance();
				return true;
			}
			return false;
		}
		bool Match(TokenType token)
		{
			return token == currentToken;
		}
		void MandatoryToken(TokenType token)
		{
			if( ! MatchAndAdvance(token) )
				throw new Exception("Parser: Bad input file.");
		}
		bool NextTokenIsIJK()
		{
			return Match(TokenType.I) || Match(TokenType.J) || Match(TokenType.K);
		}
		bool S()
		{
			//string zoneTitle = "";
			//Title Section
			if( MatchAndAdvance( TokenType.TITLE ) )
			{
				if(Match( TokenType.EQ ) )//It should be mandatory, I know...
					Advance();
				if( Scanner.IsID( currentToken ) )
				{
					mesh.HasTitle = true;
					mesh.Title = scanner.Text;
					Advance();
				}
				else throw new Exception("Parser: Bad input file.");
			}
			else if(! Match( TokenType.VARIABLES ) )
			{
				if( Scanner.IsID( currentToken ) )
				{
					mesh.HasTitle = true;
					mesh.Title = scanner.Text;
					Advance();
				}
				else throw new Exception("Parser: Bad input file.");
			}
			else mesh.HasTitle = false;
			//Variables Section
			MandatoryToken(TokenType.VARIABLES);
			MandatoryToken(TokenType.EQ);
			AddVariable();
			while( MatchAndAdvance( TokenType.COMMA ) )
				AddVariable();
			//Zone+
			if(! Match( TokenType.ZONE ) ) //The first zone is mandatory.
				throw new Exception("Parser: There should be at least one zone.");
			while( MatchAndAdvance( TokenType.ZONE ) )
			{
				//Note: There might be T = title for the zone
				if( Match( TokenType.N ) )
					FEZone();
				else if( NextTokenIsIJK() )
					IJKZone();
				else throw new Exception("Parser: Bad zone specifications.");
			}
			return true;
		}
		bool FEZone()
		{
			uint n; //Number of vertices
			uint e, faceCount, edgeCount;// e: Number of elements
			TokenType et;//Element Type: TRIANGLE, QUAD, ...
			ElementType elementType;
			TokenType feType;//FEPOINT or FEBLOCK
			Zone zone;//The zone that we will create 
			Face[] faceTable;//The face table of the zone.
			Element[] elementTable;
			Edge[] edgeTable;
			double dvalue;//A double value for reading the input file
			uint cVars;//Number of variables per vertex
		
			//N = value TokenType.
			MandatoryToken(TokenType.N);
			MandatoryToken(TokenType.EQ);
			if(! Match( TokenType.NUMBER ) )
				throw new Exception("Parser: N = bad data.");
			n = (uint)scanner.Number;
			Advance();
			//, E = value
			MandatoryToken(TokenType.COMMA);
			MandatoryToken(TokenType.E);
			MandatoryToken(TokenType.EQ);
			if(! Match( TokenType.NUMBER ) )
				throw new Exception("Parser: E = bad data.");
			e = scanner.Number;
			Advance();
			//, F = feType
			MandatoryToken(TokenType.COMMA);
			MandatoryToken(TokenType.F);
			MandatoryToken(TokenType.EQ);
			feType = currentToken; //there can be an error.
			Advance();
			//, ET = etype
			MandatoryToken(TokenType.COMMA);
			MandatoryToken(TokenType.ET);
			MandatoryToken(TokenType.EQ);
			et = currentToken; //there can be an error.
		
			faceCount = CalculateFaceCount(e, et);
			edgeCount = CalculateEdgeCount(e, et);
			elementType = GetElementType( et );
			zone = new Zone(
				elementType,
				n,
				e,
				faceCount,
				edgeCount,
				cData);
			mesh.Zones.PushBack( zone );
			//get overall number of variables per vertex. (including the coordinates)
			cVars = (uint)storerTable.Count;
			//Read the points
			if(feType == TokenType.FEPOINT)
			{
				for(uint i = 0; i < n; i++)
					foreach(VariableStorer vs in storerTable)
					{
						fstream.Read(out dvalue);
						vs.Store(ref dvalue, zone.Vertices[i]);
					}
			}
			else if(feType == TokenType.FEBLOCK)
			{
				foreach(VariableStorer vs in storerTable)
					for(uint i = 0; i < n; i++)
					{
						fstream.Read(out dvalue);
						vs.Store(ref dvalue, zone.Vertices[i]);
					}
			}
			else throw new Exception("Parser: F = Bad data format.");//ERROR

			//Read the connectivity list
			faceTable = zone.Faces;
			edgeTable = zone.Edges;
			elementTable = zone.Elements;
			uint currentEdge = 0;
			//uint currentElement = 0;
			switch(et)
			{
				case TokenType.QUADRILATERAL:
				{
					//f = e
					uint p1, p2, p3, p4;
					for(uint i = 0; i < faceCount; i++)
					{
						fstream.Read(out p1).Read(out p2).
							Read(out p3).Read(out p4);
						faceTable[i].SetQuadVertices(--p1, --p2, --p3, --p4);
						elementTable[i].SetQuad( i );
						edgeTable[currentEdge++].Set( p1, p2);
						edgeTable[currentEdge++].Set( p2, p3);
						edgeTable[currentEdge++].Set( p3, p4);
						edgeTable[currentEdge++].Set( p4, p1);
						faceTable[i].SetQuad(currentEdge - 4, currentEdge - 3,
											currentEdge - 2, currentEdge - 1);
					}
				}
					break;
				case TokenType.TRIANGLE:
				{
					//f = e
					uint p1, p2, p3;
					for(uint i = 0; i < faceCount; i++)
					{
						fstream.Read(out p1).Read(out p2).Read(out p3);
						faceTable[i].SetTriangleVertices(--p1, --p2, --p3);
						elementTable[i].SetTriangle( i );
						edgeTable[currentEdge++].Set( p1, p2);
						edgeTable[currentEdge++].Set( p2, p3);
						edgeTable[currentEdge++].Set( p3, p1);
						faceTable[i].SetTriangle(currentEdge - 3, currentEdge - 2,
							currentEdge - 1);
					}
				}
					break;
				case TokenType.TETRAHEDRON:
				{
					//f = 4 * e. That's, every element read should produce four faces.
					uint p1, p2, p3, p4;
					uint currentElement = 0;
					uint i = 0;
					while(i < faceCount)
					{
						fstream.Read(out p1).Read(out p2).
							Read(out p3).Read(out p4);

						--p1; --p2; --p3; --p4;

						elementTable[currentElement].SetTetrahedronBaseFace(i);
						elementTable[currentElement].SetTetrahedronFrontFace(i+1);
						elementTable[currentElement].SetTetrahedronRightFace(i+2);
						elementTable[currentElement++].SetTetrahedronLeftFace(i+3);

						edgeTable[currentEdge++].Set(p1, p2);//base-base
						edgeTable[currentEdge++].Set(p2, p3);//base-right
						edgeTable[currentEdge++].Set(p3, p1);//base-left
						edgeTable[currentEdge++].Set(p1, p4);//front-left
						edgeTable[currentEdge++].Set(p2, p4);//front-right
						edgeTable[currentEdge++].Set(p3, p4);//right-right

						faceTable[i].SetTriangleVertices(p1, p2, p3);//base
						faceTable[i].SetTriangle(currentEdge-6, currentEdge-5,
													currentEdge-4);
						i++;
						faceTable[i].SetTriangleVertices(p2, p3, p4);//front
						faceTable[i].SetTriangle(currentEdge-6, currentEdge-2,
							currentEdge-3);
						i++;
						faceTable[i].SetTriangleVertices(p3, p4, p1);//right
						faceTable[i].SetTriangle(currentEdge-5, currentEdge-2,
							currentEdge-1);
						i++;
						faceTable[i].SetTriangleVertices(p4, p1, p2);//left
						faceTable[i].SetTriangle(currentEdge-4, currentEdge-1,
							currentEdge-3);
						i++;
					}
				}
					break;
				case TokenType.BRICK:
				{
					//f = 6 * e. That's, every element read should produce six faces.
					uint currentElement = 0;
					uint p1, p2, p3, p4, p5, p6, p7, p8;
					uint i = 0;
					while(i < faceCount)
					{
						fstream.Read(out p1).Read(out p2).
							Read(out p3).Read(out p4).Read(out p5).
							Read(out p6).Read(out p7).Read(out p8);

						--p1; --p2; --p3; --p4;
						--p5; --p6; --p7; --p8;

                        elementTable[currentElement].vertInOrder = new uint[] {p1,p2,p3,p4,p5,p6,p7,p8 };

						edgeTable[currentEdge++].Set(p1, p2);//bottom-front
						edgeTable[currentEdge++].Set(p2, p3);//bottom-right
						edgeTable[currentEdge++].Set(p3, p4);//bottom-back
						edgeTable[currentEdge++].Set(p4, p1);//bottom-left
						edgeTable[currentEdge++].Set(p5, p6);//top-front
						edgeTable[currentEdge++].Set(p6, p7);//top-right
						edgeTable[currentEdge++].Set(p7, p8);//top-back
						edgeTable[currentEdge++].Set(p8, p5);//top-left
						edgeTable[currentEdge++].Set(p1, p5);//c1
						edgeTable[currentEdge++].Set(p2, p6);//c2
						edgeTable[currentEdge++].Set(p3, p7);//c3
						edgeTable[currentEdge++].Set(p4, p8);//c4


						elementTable[currentElement].SetBrickBottomFace(i);
						elementTable[currentElement].SetBrickTopFace(i+1);
						elementTable[currentElement].SetBrickFrontFace(i+2);
						elementTable[currentElement].SetBrickRightFace(i+3);
						elementTable[currentElement].SetBrickBackFace(i+4);
						elementTable[currentElement++].SetBrickLeftFace(i+5);
						
						faceTable[i].SetQuadVertices(p1, p2, p3, p4);//bottom
						faceTable[i].SetQuad( currentEdge - 12, currentEdge - 11,
											currentEdge - 10, currentEdge - 9);
						i++;
						faceTable[i].SetQuadVertices(p5, p6, p7, p8);//top
						faceTable[i].SetQuad( currentEdge - 8, currentEdge - 7,
							currentEdge - 6, currentEdge - 5);
						i++;
						faceTable[i].SetQuadVertices(p5, p1, p2, p6);//front
						faceTable[i].SetQuad( currentEdge - 4, currentEdge - 12,
							currentEdge - 3, currentEdge - 8);
						i++;
						faceTable[i].SetQuadVertices(p6, p2, p3, p7);//right
						faceTable[i].SetQuad( currentEdge - 3, currentEdge - 11,
							currentEdge - 2, currentEdge - 7);
						i++;
						faceTable[i].SetQuadVertices(p4, p8, p7, p3);//back
						faceTable[i].SetQuad( currentEdge - 1, currentEdge - 6,
							currentEdge - 2, currentEdge - 10);
						i++;
						faceTable[i].SetQuadVertices(p4, p1, p5, p8);//left
						faceTable[i].SetQuad( currentEdge - 9, currentEdge - 4,
							currentEdge - 5, currentEdge - 1);
						i++;
					}
				}
					break;
			}
			return true;
		}
		bool IJKZone()
		{
			/*Note: We donot handle the following errors:
	 *			1 - The user specifies the same variable more than once.
	 */
			bool formatIsBlock;
			uint imax = 1;
			uint jmax = 1;
			uint kmax = 1;
			uint kstep;
			uint jstep;
			uint cVertices;//Number of vertices
			uint cVars;//Overall number of variables in every vertex
			Zone zone;//The zone that we will create
			Element[] elementTable;
			Face[] faceTable;//The face table of the zone
			Edge[] edgeTable;
			ElementType elementType;
			uint faceCount = 0;//Number of faces in the zone
			uint elementCount = 0;//Number of elements in the zone
			uint edgeCount = 0;//Number of edges in the zone
			double dvalue;//A double value for reading the input file
			//The following data will be used to identify the dimensionality of the data
			//and may be later used to identify wether a field has been repeated in the
			//input (which indicates an error) or not.
			byte ijkFlag = 0;
			const byte IBit = 0x1;
			const byte JBit = 0x2;
			const byte KBit = 0x4;
			byte[] NumberOfBits = new byte[8] { 0, 1, 1, 2, 1, 2, 2, 3};
			//										0, 1, 2, 3, 4, 5, 6, 7
			do
			{
				switch( currentToken )
				{
					case TokenType.I:
						Advance();
						MandatoryToken( TokenType.EQ );
						if(! Match( TokenType.NUMBER ) )
							throw new Exception("Parser: I = bad data.");
						imax = (uint)scanner.Number;
						if(imax > 1) 
							ijkFlag |= IBit;
						Advance();
						break;
					case TokenType.J:
						Advance();
						MandatoryToken( TokenType.EQ );
						if(! Match( TokenType.NUMBER ) )
							throw new Exception("Parser: J = bad data.");
						jmax = (uint)scanner.Number;
						if(jmax > 1) 
							ijkFlag |= JBit;
						Advance();
						break;
					case TokenType.K:
						Advance();
						MandatoryToken( TokenType.EQ );
						if(! Match( TokenType.NUMBER ) )
							throw new Exception("Parser: K = bad data.");
						kmax = (uint)scanner.Number;
						if(kmax > 1) 
							ijkFlag |= KBit;
						Advance();
						break;
				}
				MandatoryToken( TokenType.COMMA );
			}while( NextTokenIsIJK() );
	
			//F = POINT
			//or
			//F = BLOCK
			MandatoryToken( TokenType.F );
			MandatoryToken( TokenType.EQ );
			if( Match( TokenType.POINT ) )
				formatIsBlock = false;
			else if( Match( TokenType.BLOCK ) )
				formatIsBlock = true;
			else throw new Exception("Parser: F = Bad data format.");
	
			jstep = imax;
			kstep = (uint)(jmax * imax);
			cVertices = (uint)(imax * jmax * kmax);	//Calculate number of vertices
			if( NumberOfBits[ ijkFlag ] == 1 ) //One-dimensional data
			{
				elementType = ElementType.Line;
				edgeCount = (uint)(cVertices - 1);
				//faceCount = 0;
			}
			else //Two-dimensional or Three-dimensional data
			{
				uint baseLevelQuads = (uint)((imax - 1)*(jmax - 1));
				uint cLevels = kmax;
				uint cLevelConnectiveQuads = (uint)((imax - 1) // bottom wall
					+(jmax - 1) // right wall
					+(baseLevelQuads<<1)); //2 * baseLevelQuads which
				//denotes top&left edge-extrusion
				//per quad
				faceCount = baseLevelQuads*cLevels + 
					cLevelConnectiveQuads*(cLevels - 1);
				edgeCount = (uint)(kmax * ( jmax * (imax - 1) + imax * (jmax - 1) ));
				edgeCount += (uint)((kmax - 1) * imax * jmax);
				if( NumberOfBits[ ijkFlag ] == 2 ) //2D
				{
					elementType = ElementType.IJKQuad;
					elementCount = faceCount;
				}
				else //3D
				{
					elementType = ElementType.IJKBrick;
					elementCount = (uint)((kmax - 1) * (jmax - 1) * (imax - 1));
				}
			}
			cVertices = (uint)(imax * jmax * kmax);	//Calculate number of vertices
			zone = new Zone(	elementType, //Create the zone
				cVertices,
				elementCount,
				faceCount,
				edgeCount,
				cData);
			mesh.Zones.PushBack( zone );
			//Get number of overall variables per vertex
			//Read the vertices
			cVars = (uint)storerTable.Count;
			if( formatIsBlock )
			{
				foreach(VariableStorer vs in storerTable)
					for(uint u = 0; u < cVertices; u++)
					{
						fstream.Read(out dvalue);
						vs.Store(ref dvalue, zone.Vertices[u]);
					}
			}
			else
			{
				for(uint u = 0; u < cVertices; u++)
					foreach(VariableStorer vs in storerTable)
					{
						fstream.Read(out dvalue);
						vs.Store(ref dvalue, zone.Vertices[u]);
					}
			}
			//Construct elements, faces, edges
			if(elementType == ElementType.Line)
			{
				edgeTable = zone.Edges;
				for(uint l = 0; l < edgeCount; l++)
					edgeTable[l].Set(l, (uint)(l+1));
				return true;
			}
			uint i, j, k;
			uint kv, kjv;
			uint currentElement = 0, currentEdge = 0, currentFace = 0;
			//currentEntity = 0;
			faceTable = zone.Faces;
			edgeTable = zone.Edges;
			elementTable = zone.Elements;


			//Construct edges...

			//Establish the i-line edges
			//PIndexOf(K, I, J) = K * kstep + J * jstep + I
			for(k = 0, kv = 0; k < kmax; k++, kv += kstep)
				for(j = 0, kjv = kv; j < jmax; j++, kjv += jstep)
					for(i = 0; i < imax - 1; i++)
						edgeTable[currentEdge++].Set(
							(uint)(kjv + i), //PIndexOf(k, j, i)
							(uint)(kjv + i + 1)); //PIndexOf(k, j, i+1)

			//Establish the j-line edges
			for(k = 0, kv = 0; k < kmax; k++, kv += kstep)
				for(j = 0, kjv = kv; j < jmax - 1; j++, kjv += jstep)
					for(i = 0; i < imax; i++)
						edgeTable[currentEdge++].Set(
							(uint)(kjv + i), //PIndexOf(k, j, i)
							(uint)(kjv + jstep + i)); //PIndexOf(k, j+1, i)

			//Establish the k-line edges
			for(k = 0, kv = 0; k < kmax - 1; k++, kv += kstep)
				for(j = 0, kjv = kv; j < jmax; j++, kjv += jstep)
					for(i = 0; i < imax; i++)
						edgeTable[currentEdge++].Set(
							(uint)(kjv + i), //PIndexOf(k, j, i)
							(uint)(kjv + kstep + i)); //PIndexOf(k+1, j, i)


			//Establish the faces

			//For every point in the base jk-level, there are  be imax - 1 edges 
			//in front of it parallel to the i-axis.
			uint jLineEdgesStartingIndex = (uint)(jmax * kmax * (imax - 1)); // = Number of i-line edges
			//For every point in the base ki-level, there are be jmax - 1 edges 
			//above it parallel to the j-axis. 
			uint kLineEdgesStartingIndex = (uint)(jLineEdgesStartingIndex // = Number of i-line edges
											+ kmax * imax * (jmax - 1));// + Number of j-line edges
			uint iedgeKV, iedgeKJV;// Index of the edge parallel to the i-axis whose start point is at (0, J, K)
			uint jedgeKV, jedgeKJV;// Index of the edge parallel to the j-axis whose start point is at (0, J, K)
/*			
 *			uint kedgeKV, kedgeKJV;// Index of the edge parallel to the i-axis whose index is K, J
			//The kedgeKV is the same as kv and so is the kjv version.			
*/
			uint iEdgeJstep, iEdgeKstep;
			uint jEdgeJstep, jEdgeKstep;
//			uint j_KEdgeStep, k_KEdgeStep;
			/*
			 The VEdgeUstep:	The step needed to be multiplied by the U variable in 
								order to get the index of the edge parallel to the V-axis.
								This index is used in the Uth dimension of the array.
								where:
								1) U belongs to the set {j, k}. i is excluded since the 
								ith dimension of the array has its step = 1.
								2) V belongs to the set {I, J}. K is excluded since all
								edges parallel to the K-axis has their jEdgeStep
								= jstep = imax, and their kEdgeStep = kstep 
								= jmax * jstep.
								Summary:
								1) V represents the axis to which the edges are parallel.
								2) U represents the axis over which we take the step.
			 */
			iEdgeJstep = (uint)(imax - 1);
			iEdgeKstep = (uint)(jmax*(iEdgeJstep));
			jEdgeJstep = (uint)imax;
			jEdgeKstep = (uint)((jmax-1)*jEdgeJstep);
			//Establish the ij-levels faces
			for(k = 0, kv = 0, iedgeKV = 0, jedgeKV = 0;
				k < kmax;
				k++, kv += kstep, iedgeKV += iEdgeKstep, jedgeKV += jEdgeKstep)
			{
				for(j = 0, kjv = kv, iedgeKJV = iedgeKV, jedgeKJV = jedgeKV;
					j < jmax - 1;
					j++, kjv += jstep, iedgeKJV += iEdgeJstep, jedgeKJV += jEdgeJstep)
					for(i = 0; i < imax - 1; i++)
					{
						/*faceTable[currentFace].SetQuad(
							(uint)(kjv + i), //EIndexOf(k, j, i, bottom)
							(uint)(kjv + i + 1 + jLineEdgesStartingIndex), //EIndexOf(k, j, i+1, right)
							(uint)(kjv + jstep + i), //EIndexOf(k, j+1, i+1, top)
							(uint)(kjv + i	+ jLineEdgesStartingIndex)); //EIndexOf(k, j+1, i, left)*/
						faceTable[currentFace].SetQuad(
							(uint)(iedgeKJV + i), //EIndexOf(k, j, i, bottom)
							(uint)(jedgeKJV + i + 1 + jLineEdgesStartingIndex), //EIndexOf(k, j, i+1, right)
							(uint)(iedgeKJV + iEdgeJstep + i), //EIndexOf(k, j+1, i+1, top)
							(uint)(jedgeKJV + i + jLineEdgesStartingIndex)); //EIndexOf(k, j+1, i, left)
						faceTable[currentFace++].SetQuadVertices(
							(uint)(kjv + i), 
							(uint)(kjv + i + 1), 
							(uint)(kjv + jstep + i + 1), 
							(uint)(kjv + jstep + i ));
					}
			}
			//Establish the jk-levels
			for(k = 0, kv = 0, jedgeKV = 0;
				k < kmax - 1;
				k++, kv += kstep, jedgeKV += jEdgeKstep)
			{
				for(j = 0, kjv = kv, jedgeKJV = jedgeKV;
					j < jmax - 1;
					j++, kjv += jstep, jedgeKJV += jEdgeJstep)
					for(i = 0; i < imax; i++)
					{
						faceTable[currentFace].SetQuad(
							(uint)(kjv + i + kLineEdgesStartingIndex),	//EIndexOf(k, j, i, bottom)
							(uint)(jedgeKJV + jEdgeKstep + i + jLineEdgesStartingIndex), //EIndexOf(k, j, i+1, left)
							(uint)(kjv + jstep + i + kLineEdgesStartingIndex),//EIndexOf(k, j+1, i+1, top)
							(uint)(jedgeKJV + i + jLineEdgesStartingIndex)); //EIndexOf(k, j, i, right)	
						faceTable[currentFace++].SetQuadVertices(
							(uint)(kjv + i + kstep), 
							(uint)(kjv + i), 
							(uint)(kjv + jstep + i), 
							(uint)(kjv + kstep + jstep + i ));
					}
			}
			//Establish the ki-levels
			for(k = 0, kv = 0, iedgeKV = 0;
				k < kmax - 1;
				k++, kv += kstep, iedgeKV += iEdgeKstep)
			{
				for(j = 0, kjv = kv, iedgeKJV = iedgeKV;
					j < jmax;
					j++, kjv += jstep, iedgeKJV += iEdgeJstep)
					for(i = 0; i < imax - 1; i++)
					{
						faceTable[currentFace].SetQuad(
							(uint)(iedgeKJV + i),	//EIndexOf(k, j, i, top)
							(uint)(kjv + i + 1 + kLineEdgesStartingIndex), //EIndexOf(k, j, i+1, right)
							(uint)(iedgeKJV + iEdgeKstep + i),//EIndexOf(k, j+1, i+1, top)
							(uint)(kjv + i	+ kLineEdgesStartingIndex)); //EIndexOf(k, j, i, right)
						faceTable[currentFace++].SetQuadVertices(
							(uint)(kjv + i), 
							(uint)(kjv + i + 1), 
							(uint)(kjv + kstep + i + 1), 
							(uint)(kjv + kstep + i ));
					}
			}
			//Establish the elements
			if(elementType == ElementType.IJKBrick)
			{
				uint jkFaceStart = (uint)(kmax*(imax - 1)*(jmax - 1));
				uint kiFaceStart = (uint)(jkFaceStart
					+ imax*(jmax - 1)*(kmax - 1));
				jstep = imax;
				jstep--;
				kstep = (uint)((jmax-1)*jstep);
				for(k = 0, kv = 0; k < kmax - 1; k++, kv += kstep)
				{
					for(j = 0, kjv = kv; j < jmax - 1; j++, kjv += jstep)
					{
						for(i = 0; i < imax - 1; i++)
						{
							elementTable[currentElement].SetBrickFrontFace((uint)(kjv + kstep + i));
							elementTable[currentElement].SetBrickRightFace((uint)(k*(jmax-1)*imax + j*imax + i + 1 + jkFaceStart));
							elementTable[currentElement].SetBrickBackFace((uint)(kjv + i));
							elementTable[currentElement].SetBrickLeftFace((uint)(k*(jmax-1)*imax + j*imax + i + jkFaceStart));
							elementTable[currentElement].SetBrickTopFace((uint)(k*jmax*(imax-1) + j*(imax-1) + jstep + i + kiFaceStart));
							elementTable[currentElement].SetBrickBottomFace((uint)(k*jmax*(imax-1) + j*(imax-1) + i + kiFaceStart));

                            elementTable[currentElement].vertInOrder = new uint[8];

                            elementTable[currentElement].vertInOrder[0] = zone.Faces[elementTable[currentElement].GetBrickBottomFace()].Vertices[3];
                            elementTable[currentElement].vertInOrder[1] = zone.Faces[elementTable[currentElement].GetBrickBottomFace()].Vertices[2];
                            elementTable[currentElement].vertInOrder[2] = zone.Faces[elementTable[currentElement].GetBrickBottomFace()].Vertices[1];
                            elementTable[currentElement].vertInOrder[3] = zone.Faces[elementTable[currentElement].GetBrickBottomFace()].Vertices[0];

                            elementTable[currentElement].vertInOrder[4] = zone.Faces[elementTable[currentElement].GetBrickTopFace()].Vertices[3];
                            elementTable[currentElement].vertInOrder[5] = zone.Faces[elementTable[currentElement].GetBrickTopFace()].Vertices[2];
                            elementTable[currentElement].vertInOrder[6] = zone.Faces[elementTable[currentElement].GetBrickTopFace()].Vertices[1];
                            elementTable[currentElement].vertInOrder[7] = zone.Faces[elementTable[currentElement].GetBrickTopFace()].Vertices[0];

                            currentElement++;
						}
					}
				}
			}
			else //element type is IJK quad
			{
				for(i = 0; i < faceCount; i++)
					elementTable[currentElement++].SetQuad( i );
			}
			return true;
		}
		static uint FaceTypeOf(TokenType feET)
		{
			switch(feET)
			{
				case TokenType.TRIANGLE:
				case TokenType.TETRAHEDRON:
					return Gl.GL_TRIANGLES;
				case TokenType.QUADRILATERAL:
				case TokenType.BRICK:
					return Gl.GL_QUADS;
				default:
					return 0; //Error
			}
		}
		static uint CalculateFaceCount(uint cElements, TokenType elementType)
		{
			switch(elementType)
			{
				case TokenType.TRIANGLE:
				case TokenType.QUADRILATERAL:
					return cElements;
				case TokenType.TETRAHEDRON:
					return cElements<<2;//cElements * 4
				case TokenType.BRICK:
					return cElements*6;//cElements<<2 + cElements<<1
				default:
					return 0; //Error
			}
		}
		static uint CalculateEdgeCount(uint cElements, TokenType elementType)
		{
			switch(elementType)
			{
				case TokenType.TRIANGLE:
					return cElements*3;
				case TokenType.QUADRILATERAL:
					return cElements<<2;//cElements * 4
				case TokenType.TETRAHEDRON:
					return cElements*6;//cElements<<2 + cElements<<1
				case TokenType.BRICK:
					return cElements*12;//cElements<<3 + cElements<<2
				default:
					return 0; //Error
			}
		}
		static ElementType GetElementType(TokenType elementType)
		{
			switch(elementType)
			{
				case TokenType.TRIANGLE:
					return ElementType.Triangle;
				case TokenType.QUADRILATERAL:
					return ElementType.FEQuad;
				case TokenType.TETRAHEDRON:
					return ElementType.Tetrahedron;
				case TokenType.BRICK:
					return ElementType.FEBrick;
				default:
					return 0; //Error
			}
		}
		uint					cData;
		ArrayList				storerTable;
		TokenType				currentToken;
		Mesh					mesh;
		Scanner					scanner;
		Stream					fin;
		FormattedStream			fstream;
	
		public Parser()
		{
			fstream = new FormattedStream();
			scanner = new Scanner();
			storerTable = new ArrayList();
			cData = 0;
		}

		public void LoadMesh(string fileName, Mesh m)
		{
			FileStream fstream = new FileStream(fileName, FileMode.Open);
			LoadMesh(fstream, m);
			fstream.Close();
		}

		public void LoadMesh(Stream stream, Mesh m)
		{
			mesh = m;
			fin = stream;
			fstream.Reset(stream);
			scanner.Reset(stream);
			Advance();
			storerTable.Clear();
			cData = 0;
			if (!S())
				throw new Exception("Unable to parse input file.");
		}
	}
}