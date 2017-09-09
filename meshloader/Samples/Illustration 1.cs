using System;
using System.Collections;
using Visualization;

namespace Samples
{
	class Illustration
	{
		static void Main()
	{
		string fileName = "";
		double maxDimLength = 1.0;
		bool loaded = false;
		Mesh mesh = null;

		//Try loading the data file
		while (!loaded)
		{
			try
			{
				Console.Write("File name: ");
				fileName = Console.ReadLine(); //Get the file name

				mesh = new Mesh(fileName, maxDimLength); //Try loading the file
				loaded = true;
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}


		int numberOfZones = mesh.Zones.Count;

		Console.WriteLine("The data file has {0} zone(s).", numberOfZones);
		
		
		//List the variable names, their indices, minimum and maximum values
		Console.WriteLine(
			"{0, -20} {1, -10} {2, -12} {3, -12}",
			"Variable Name",
			"Index",
			"Minimum",
			"Maximum");

		foreach(DictionaryEntry entry in mesh.VarToIndex)
		{
			string varName = (string)entry.Key;
			uint index = (uint)entry.Value;
			double minVal, maxVal;

			mesh.GetMinMaxValues(index, out minVal, out maxVal);

			Console.WriteLine(
				"{0, -20} {1, -10} {2, -12} {3, -12}",
				varName,
				index,
				minVal,
				maxVal
				);
		}

		//find and print the face with maximum perimeter
		int maxZone, maxFace;
		double maxPerimeter;
		MeshOps.FindMaximumFacePerimeter(mesh, out maxZone, out maxFace, out maxPerimeter);
		Console.WriteLine(
			"The face with maximum perimeter: zone index: {0}, face index: {1}, perimeter: {2}",
			maxZone,
			maxFace,
			maxPerimeter);

		Console.ReadKey(true); 
	}
	}
}