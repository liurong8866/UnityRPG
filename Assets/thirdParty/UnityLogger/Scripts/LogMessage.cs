using UnityEngine;
using System.Collections;
using System;

public class LogMessage
{
	public float time;
	public int frame;
	public Log.Category category;
	public Log.Level level;
	public string message;
	public string stack;
	public int occurances;
	
	public LogMessage( float time, int frame, Log.Category category, Log.Level level, string message, string stack )
	{
		this.time = time;
		this.frame = frame;
		this.category = category;
		this.level = level;
		this.message = ( message == null ) ? "" : message;
		this.stack = ( stack == null ) ? "" : stack;
		this.occurances = 1;
	}
	
	public string ToCSV( )
	{
		return string.Format( "{0:F2},{1:F0},{2},{3},\"{4}\",\"{5}\"", time, frame, category.ToString( ), 
			level.ToString( ), SanitizeString( message ), SanitizeString( stack ) );
	}
	
	private string SanitizeString( string target )
	{
		return target.Replace( "\"", "'" );
	}
}
