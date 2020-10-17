using UnityEngine;
using System;

[System.Serializable]
public class ArrayLayout 
{

	[System.Serializable]
	public struct rowData
	{
		public int[] row;
	}

	public rowData[] rows = new rowData[10]; //Grid of 10x10
}
