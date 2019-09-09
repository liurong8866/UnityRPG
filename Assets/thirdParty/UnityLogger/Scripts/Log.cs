#if UNITY_EDITOR
#define DEBUG
#endif

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class Log
{
	public enum Level { Trivial = 0, Minor = 1, Normal = 2, Important = 3, Critical = 4 };
	[Flags]	public enum Category { Console = 1, System = 2, GUI = 4, AI = 8, Network = 16, Physics = 32, General = 64, Animation = 128, Sound = 256, Camera = 512 };
	[Flags]	public enum Options { UseConsole = 1, ShowPrefix = 2 };
	
	private List<LogMessage> messages, current;
	private Action action;
	private int messageQueueSize = 8196;
	private bool useConsole = false, interceptOn = true;
	private Dictionary<string,string> watches;
	
	public int Count 
	{
		get
		{
			return messages.Count;
		}
	}
	
	private static readonly Log _instance = new Log( );
	public static Log Instance 
	{
		get
		{
			return _instance;
		}
	}
	
	private Log( )
	{
		messages = new List<LogMessage>( messageQueueSize );
		current = new List<LogMessage>( );
		watches = new Dictionary<string, string>( );
	}
	
	/** Convenience Methods
	 * 
	 *  Logs a message to the appropriate level, uses the given category and message.  Overloaded method
	 *  will assume Category.Default, each of these methods will invoke Record with appropriate parameters
	 *  to complete logging.
	 */
	public static void Trivial( string message ) { Trivial( Category.Console, message ); }
	public static void Trivial( Category category, string message )
	{
		Record( category, Level.Trivial, message );
	}

	public static void Minor( string message ) { Minor( Category.Console, message ); }
	public static void Minor( Category category, string message )
	{
		Record( category, Level.Minor, message );
	}

	public static void Normal( string message ) { Normal( Category.Console, message ); }
	public static void Normal( Category category, string message )
	{
		Record( category, Level.Normal, message );
	}
    [Conditional("DEBUG")]
	public static void AI(string messages) {
		Important (Category.AI, messages);
	}
    [Conditional("DEBUG")]
	public static void Ani(string message) {
		Important (Category.Animation, message);
	}
    [Conditional("DEBUG")]
	public static void Sys(string messages) {
		Important (Category.System, messages);
	}
    [Conditional("DEBUG")]
	public static void GUI(string message) {
		Important (Category.GUI, message);
	}
    [Conditional("DEBUG")]
	public static void Net(string message) {
		Important (Category.Network, message);
	}

    [Conditional("DEBUG")]
	public static void Important( string message ) { Important( Category.Console, message ); }

    [Conditional("DEBUG")]
	public static void Important( Category category, string message )
	{
		Record( category, Level.Important, message );
	}

    [Conditional("DEBUG")]
	public static void Critical( string message ) { Critical( Category.Console, message ); }
    [Conditional("DEBUG")]
	public static void Critical( Category category, string message )
	{
		Record( category, Level.Critical, message );
	}
	
	/** Record
	 * 
	 *  Actual recording of log message is done here, we will first check if the useConsole override
	 *  is in effect and if so just spit to console with a prefix.  
	 */
	public static void Record( Category category, Level level, string message )
	{
		Record( new LogMessage( Time.time, Time.frameCount, category, level, message,
			UnityEngine.StackTraceUtility.ExtractStackTrace( ) ) );
	}
	
	public static void Record( Category category, Level level, string message, string stack )
	{
		Record( new LogMessage( Time.time, Time.frameCount, category, level, message, stack ) );
	}
	
	public static void Record( LogMessage logMessage )
	{
		if( Instance.useConsole )
		{
			UnityEngine.Debug.Log( logMessage.message );
		}
		else
		{
			if( Instance.messages.Count > 0 )
			{
				// check if this log message is the same as the previous log message, if so just up occurances
				LogMessage lastEntry = Instance.messages[ Instance.messages.Count - 1 ];
				string newMessage = logMessage.message + logMessage.stack;
				string oldMessage = lastEntry.message + lastEntry.stack;
					
				if( newMessage == oldMessage && lastEntry.category == logMessage.category && 
					lastEntry.level == logMessage.level )
				{
					// increment the number of occurances for this item
					lastEntry.occurances ++;
					
					// if we have the log viewer attached as a listener, send a message something changed
					if( Instance.action != null )
					{
						Instance.action.Invoke( );
					}
					
					return;
				}
			}
			
			// add latest log message to queue
			Instance.messages.Add( logMessage );
			if( Instance.messages.Count > Instance.messageQueueSize )
			{
				Instance.messages.RemoveAt( 0 );
                try {
				    Instance.action.Invoke( );
                }catch(Exception ex) {
                    
                }
			}
		}
	}
	
	public static void SetListener( Action action )
	{
		Instance.action = action;
	}
	
	public int FilterLogs( Log.Category categories, Log.Level level, string keyword, int lockedAt = -1 )
	{
		// ensure we never have a null parameter
		if( keyword == null )
		{
			keyword = string.Empty;
		}
		
		// using the lockedAt parameter, determine if we want all or a partial set of messages
		int safeLockedAt;
		if( lockedAt < 1 )
		{
			safeLockedAt = messages.Count;
		}
		else
		{
			safeLockedAt = Mathf.Clamp( lockedAt, lockedAt, messages.Count );
		}
		
		// fetch the list for a given range
		current = messages.GetRange( 0, safeLockedAt ).FindAll( ( LogMessage message ) => ( 
			categories & message.category ) > 0 && message.level >= level && message.message.ToLower( ).Contains( keyword.ToLower( ) ) 
		);
		
		current.Reverse( );
		return current.Count;
	}
	
	public int TotalLogCount( )
	{
		return messages.Count;
	}
	
	public List<LogMessage> GetRange( int position, int pageSize )
	{
		// check the size of results are > PAGE_SIZE, if not then just return them all
		if( current.Count <= pageSize )
		{
			return current;
		}
		
		// ok we have more elements than PAGE_SIZE, check if we will go past EoL with current position
		if( position > current.Count - pageSize )
		{
			position = current.Count - pageSize;
		}
		
		// finally, ensure position is never out of index range
		position = Mathf.Max( 0, position );
		
		return current.GetRange( position, pageSize );

	}
	
	public List<LogMessage> GetAllLogs( )
	{
		return Instance.messages;
	}
	
	public void SetIntercept( bool allowIntercepts )
	{
		interceptOn = allowIntercepts;
	}
	
	public bool IsInterceptOn( )
	{
		return interceptOn;
	}
	
	public bool IsConsoleOn( )
	{
		return useConsole;
	}
	
	public void Clear( )
	{
		messages.Clear( );
	}

	public void SetConsole( bool redirectToConsole )
	{
		useConsole = redirectToConsole;
	}
	
	public static void Watch( string nvpKey, object nvpValue )
	{
		if( nvpValue != null )
		{
			Instance.watches[ nvpKey.ToUpper( ) ] = nvpValue.ToString( );
		}
		else
		{
			Instance.watches[ nvpKey.ToUpper( ) ] = "<null>";
		}
	}
	
	public Dictionary<string,string> GetAllWatches( )
	{
		return watches;
	}
}
