﻿using UnityEngine;
using System.Collections;
using System;

public class AssertionFailedException : Exception 
{
	public AssertionFailedException(string message) : base(message) { }
}

public static class Assert  {

	public static void Check(bool boolean, string error) 
	{
		if (!boolean) 
		{
			string stacktrace = System.Environment.StackTrace;
			string[] lines = stacktrace.Split ('\n');
			string line = lines [2];
			int id = line.IndexOf (')');
			throw new AssertionFailedException ("[" + line.Remove(id + 1).Substring(6) + "] " + error);
		}
	}
}
