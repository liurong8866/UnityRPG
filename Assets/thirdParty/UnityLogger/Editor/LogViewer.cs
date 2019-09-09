#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
	#define UNITY_4
#endif

#if UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
	#define UNITY_3
#endif

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;

[InitializeOnLoad]
public class LogViewer : EditorWindow 
{
	private static string FILE_NAME = "log.options";

	private Dictionary<Log.Level,StateColour> levelColours;	
	
	private float logLevel = 5f;
	private Log.Category category, prevCategory;
	private Log.Level level, prevLevel;
	private string keyword, prevKeyword;
	private float scrollPosition;
	private LogMessage activeMessage;
	private GUIStyle verticalScrollbarStyle, titleBarStyle, detailsScrollviewStyle, boxStyle;
	private GUIStyle numberStyle, logStyle, selectedStyle, messageStyle, messageDetailStyle;
	private GUIStyle numberStyleSelected, boxStyleSelected, messageDetailStyleSelected;
	private GUIStyle defaultScrollStyle, pausedScrollStyle, scrollStyle;
	private bool stylesInitialized;
	private StateColour defaultTextColour;
	private bool isDirty;
	private int logSize;
	private List<LogMessage> pauseFrame;	
	private List<LogMessage> logMessages;
	
	private static LogWatcher logWatch;
	public LogProxy logProxy;
	
    [MenuItem ("Window/Log Viewer")]
    static void Init () 
	{
		// build the log viewer window and set title
        LogViewer window = ( LogViewer )EditorWindow.GetWindow( typeof( LogViewer ) );
		window.title = "Log Viewer";
		logWatch = null;
	}
	
	void OnEnable( )
	{
		// instantiate a new instance of log proxy for use by viewer
		logProxy = new LogProxy( );
		
		// setup initial filter parms and their previous values
		if( levelColours == null )
		{
			category = ( Log.Category )0xFFFFFF;
			prevCategory = category;
			
			level = Log.Level.Trivial;
			prevLevel = level;
			logLevel = 0;
			pauseFrame = null;
	
			keyword = "";
			
			Log.SetListener( OnLogChange );
			
			scrollPosition = 0f;
			activeMessage = null;
			stylesInitialized = false;
			logSize = Log.Instance.Count;
			isDirty = true;
			
			levelColours = new Dictionary<Log.Level, StateColour>( );
			LoadSettings( );
		}
		
		// assign application hook to register callbacks for console output and log them if needed
        Application.RegisterLogCallback( HandleLog );
	}
	
	void OnLogChange( )
	{
		isDirty = true;
	}
	
	void OnDisable( )
	{
        Application.RegisterLogCallback( null );
	}
	
	void OnInspectorUpdate( )
	{
		// clear style initialization
		stylesInitialized = false;
		
		// check each of the filter values for a change, if found then update the search criteria
		if( category != prevCategory )
		{
			isDirty = true;
			prevCategory = category;
			pauseFrame = null;
		}
		
		if( level != prevLevel )
		{
			isDirty = true;
			prevLevel = level;
			pauseFrame = null;
		}
		
		if( keyword != prevKeyword )
		{
			isDirty = true;
			prevKeyword = keyword;
			pauseFrame = null;
		}
		
		if( logSize != Log.Instance.Count )
		{
			isDirty = true;
		}
			
		if( isDirty )
		{
			isDirty = false;
			logSize = Log.Instance.FilterLogs( category, level, keyword );
			Repaint( );
		}
	}	
	
    void OnGUI( )
	{
		// check if we need to initialize
		if( !stylesInitialized )
		{
			InitializeColors( );
			InitializeStyles( );			
		}
		
		// render menu options and keyword filtering header
		RenderHeader( );

		// create a row banding counter for even/odd row backgrounds and determine appropriate page size
		int pageSize = Mathf.RoundToInt( Mathf.Max( 0, Screen.height - 52f ) / 18f );

		// fetch a list of all messages
		if( pauseFrame == null )
		{
			logMessages = Log.Instance.GetRange( Mathf.RoundToInt( scrollPosition ), pageSize );
			RenderBody( logMessages, pageSize );
		}
		else
		{
			RenderBody( pauseFrame, pageSize );
		}
		
		// handle keys and mouse events for scrolling through and pausing the log
		HandleKeys( );
		GUILayout.EndVertical( );
		GUILayout.EndHorizontal( );
	}
	
	private void RenderHeader( )
	{
		// build the title bar with all filtering options
		GUILayout.BeginHorizontal( titleBarStyle, GUILayout.Height( 32f ) );
		
		// render out the available category buttons and prefix with all/none
		GUILayout.Label( "Category:", GUILayout.Width( 68f ) );
		category = ( Log.Category )EditorGUILayout.EnumMaskField( category );
		GUILayout.Space( 8f );

		// display a level slider we can adjust the log level with
		GUILayout.Label( "Log Level:", GUILayout.Width( 68f ) );
		logLevel = GUILayout.HorizontalSlider( logLevel, 0f, 4f, GUILayout.Width( 128f ) );
		logLevel = Mathf.Clamp( Mathf.RoundToInt( logLevel ), 0, 4 );
		level = ( Log.Level )Mathf.RoundToInt( logLevel );
		
		string label = level.ToString( );
		GUILayout.Label( "(" + label + ")" );
		GUILayout.FlexibleSpace( );
		
		// render out the text search filter area
		GUILayout.Label( "Keyword:" );
		keyword = GUILayout.TextField( keyword == null ? "" : keyword, GUILayout.Width( 128f ) );
		GUILayout.Space( 4f );
		
 		if( GUILayout.Button( "Actions", EditorStyles.toolbarDropDown ) ) 
		{
	        GenericMenu toolsMenu = new GenericMenu( );
			toolsMenu.AddItem( new GUIContent( "Copy Active Log" ), false, CopyActiveLog );
			
			string pauseMenuText = pauseFrame == null ? "Pause Log Viewer" : "Resume Log Viewer";
			toolsMenu.AddItem( new GUIContent( pauseMenuText ), false, PauseLog );	
			toolsMenu.AddSeparator( "" );
			
			toolsMenu.AddItem( new GUIContent( "Redirect To Console" ), Log.Instance.IsConsoleOn( ), RedirectToConsole );
			toolsMenu.AddItem( new GUIContent( "Intercept Console" ), Log.Instance.IsInterceptOn( ), InterceptConsole );
			toolsMenu.AddSeparator( "" );
			
			string toggleWatches = ( logWatch == null ) ? "Show Watches" : "Hide Watches";
			toolsMenu.AddItem( new GUIContent( toggleWatches ), false, ToggleWatchWindow );
			toolsMenu.AddItem( new GUIContent( "Export to 'log.csv'" ), false, ExportToCSV );						
			toolsMenu.AddSeparator( "" );
			
			toolsMenu.AddItem( new GUIContent( "Clear Log" ), false, ClearLog );
 			toolsMenu.DropDown( new Rect( position.width - 200f, 16f, 0, 16 ) );
		}
		
		GUILayout.Space( 2f );
		GUILayout.EndHorizontal( );
	}
	
	private void RenderBody( List<LogMessage> messages, int pageSize )
	{
		// render out the log entries into a scrollable viewport
		GUILayout.BeginHorizontal( GUIStyle.none, GUILayout.ExpandHeight( true ) );
		
		// build a style for the scrollbar which represents pause state (red vs normal)
		scrollStyle = defaultScrollStyle;
		if( pauseFrame != null )
		{
			scrollStyle = pausedScrollStyle;
		}
		
		// now handle the custom scrollbar, it needs to stay in sync with the underlying log data
		scrollPosition = GUILayout.VerticalScrollbar( scrollPosition, 1f, 0f, 
			logSize - ( pageSize - 1 ), scrollStyle, GUILayout.Height( Screen.height - 52f ), GUILayout.Width( 16f ) );

		GUILayout.BeginVertical( GUIStyle.none, GUILayout.ExpandHeight( true ), GUILayout.ExpandWidth( true ) );

		// calculate total height available as pagesize * row height (18f)
		float heightRemaining = pageSize * 18f;
		float estimatedHeight = 18f;
		
		if( activeMessage != null )
		{
			// determine estimated height of fully expanded active message, remove from total height left
			string fullMessage = activeMessage.message;
			if( activeMessage.stack != null && activeMessage.stack.Length > 0 )
			{
				fullMessage += "\n" + activeMessage.stack;
			}
			
			estimatedHeight = GUI.skin.box.CalcHeight( new GUIContent( fullMessage ), position.width - 185f );
			estimatedHeight = Mathf.Max( estimatedHeight, 36f );
		}
		
		// finally, determine how much space we have for non-selected rows
		heightRemaining -= estimatedHeight;
		
		// render the log messages based on current filter parameters
		int rowCount = 0;
		bool activeMessageRendered = false;
		foreach( LogMessage logMessage in messages ) 
		{
			// assume default of grey background, check level to see if this should be overridden
			Color levelColour;
			if( levelColours.ContainsKey( logMessage.level ) )
			{
				levelColour = levelColours[ logMessage.level ].normal;
			}
			else
			{
				levelColour = defaultTextColour.normal;
			}		
			
			// set foreground text colours
			boxStyle.normal.textColor = levelColour;
			numberStyle.normal.textColor = levelColour;
			messageStyle.normal.textColor = levelColour;
			
			// check if we have reached max space allocation for rows and should blit expanded row now
			if( heightRemaining < 18f && activeMessage != null && !activeMessageRendered )
			{
				// decide what colour to render this row in, assume default if no other selections
				if( levelColours.ContainsKey( activeMessage.level ) )
				{
					RenderActiveRow( activeMessage, estimatedHeight );
				}
				else
				{
					// choose white or black for default, depending on pro vs free version of unity3d
					if( EditorGUIUtility.isProSkin )
					{
						RenderActiveRow( activeMessage, estimatedHeight );
					}
					else
					{
						RenderActiveRow( activeMessage, estimatedHeight );
					}
				}
				
				break;
			}
			else
			if( activeMessage == logMessage )
			{
				// here we have reached the active message, so render it differently (expanded)
				RenderActiveRow( logMessage, estimatedHeight );
				activeMessageRendered = true;
			}
			else
			{
				// adjust background depending on even or odd row
				logStyle.normal.background = ( rowCount % 2 == 0 ) ? logProxy.evenBackground : logProxy.oddBackground;
				
				// build a row to render out details with abbreviated text if too long
				string sanitizedMessage = logMessage.message.Split( '\n' )[ 0 ];
				Rect bounds = EditorGUILayout.BeginHorizontal( logStyle, GUILayout.Height( 18f ), GUILayout.Width( position.width ) );
				GUILayout.Label( logMessage.category.ToString( ), boxStyle, GUILayout.Width( 72f ), GUILayout.ExpandHeight( true ) );
				GUILayout.Label( logMessage.time.ToString( "F2" ), numberStyle, GUILayout.Width( 96f ), GUILayout.ExpandHeight( true ) );

				// blit out row but check if we have an occurance count to render
				if( logMessage.occurances > 1 )
				{
					GUILayout.Label( sanitizedMessage, messageStyle, GUILayout.Width( position.width - 246f ) );
					GUILayout.Space( 4f );
					GUILayout.Label( logMessage.occurances.ToString( ), numberStyle, GUILayout.Width( 48f ) );
				}
				else
				{
					GUILayout.Label( sanitizedMessage, messageStyle, GUILayout.Width( position.width - 154f ) );
				}

				EditorGUILayout.EndHorizontal( );
				
				// check for a click to expand this log entry
				if( GUI.Button( bounds, GUIContent.none, GUIStyle.none ) )
				{
					activeMessage = logMessage;
					pauseFrame = logMessages;
				}
				
				heightRemaining -= 18f;
			}
			
			rowCount ++;
		}
	}
	
	private void RenderActiveRow( LogMessage logMessage, float estimatedHeight )
	{
		// set the content colour
		StateColour levelColour = GetStateColorByLevel( logMessage.level );
		messageDetailStyle.normal.textColor = levelColour.selected;
		messageDetailStyleSelected.normal.textColor = levelColour.selected;
		boxStyleSelected.normal.textColor = levelColour.selected;
		numberStyleSelected.normal.textColor = levelColour.selected;
		boxStyle.normal.textColor = levelColour.selected;
		numberStyle.normal.textColor = levelColour.selected;
		
		// estimate the height needed for our label, subtract sum of padding + width for two lhs columns
		string fullMessage = logMessage.message;
		if( logMessage.stack != null && logMessage.stack.Length > 0 )
		{
			fullMessage += "\n" + logMessage.stack;
		}
		
		// fetch bounds to make it clickable
		GUILayout.BeginVertical( selectedStyle, GUILayout.Width( position.width ), GUILayout.Height( estimatedHeight ) );
		Rect bounds = EditorGUILayout.BeginHorizontal( GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );

		EditorGUILayout.BeginVertical( GUIStyle.none, GUILayout.Width( 72f ) );
			GUILayout.Space( 2f );
			GUILayout.Label( logMessage.category.ToString( ), boxStyleSelected );
			GUILayout.Label( logMessage.level.ToString( ), boxStyle );
		EditorGUILayout.EndVertical( );
		
		EditorGUILayout.BeginVertical( GUIStyle.none, GUILayout.Width( 96f ) );
			GUILayout.Space( 2f );
			GUILayout.Label( logMessage.time.ToString( "F2" ), numberStyleSelected, GUILayout.Width( 96f ) );
			GUILayout.Label( logMessage.frame.ToString( "F0" ), numberStyle, GUILayout.Width( 96f ) );
		EditorGUILayout.EndVertical( );
		
		EditorGUILayout.BeginVertical( GUIStyle.none, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( logMessage.message, messageDetailStyleSelected, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
			GUILayout.Label( logMessage.stack, messageDetailStyle, GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) );
		EditorGUILayout.EndVertical( );
		
		EditorGUILayout.EndHorizontal( );
		GUILayout.EndVertical( );

		// check if the user wants to close this expanded view or not
		if( GUI.Button( bounds, GUIContent.none, GUIStyle.none ) )
		{
			activeMessage = null;
			pauseFrame = null;
		}
	}
	
	private void ClearLog( )
	{
		Log.Instance.Clear( );
		isDirty = true;
	}
		
	private void PauseLog( )
	{
		if( pauseFrame == null )
		{
			pauseFrame = logMessages;
		}
		else
		{
			pauseFrame = null;
		}
	}
	
	private void CopyActiveLog( )
	{
		if( activeMessage != null )
		{
			EditorGUIUtility.systemCopyBuffer = activeMessage.message + "\n\n" + activeMessage.stack;
		}
	}
	
	private void ExportToCSV( )
	{
		string fileName = "log.csv";
		StreamWriter fileStream = null;
		
		try
		{
			// open a new filestream with write access
			fileStream = new StreamWriter( fileName, false );
			
			// write out the header
			fileStream.WriteLine( "Time,Frame,Category,Level,Message,Stack" );
			
			// write each record out
			foreach( LogMessage message in Log.Instance.GetAllLogs( ) )
			{
				fileStream.WriteLine( message.ToCSV( ) );
			}
			
			Log.Important( Log.Category.System, "Log Exported to: " + Path.GetFullPath( fileName ) );
		}
		catch( IOException x )
		{
			Log.Critical( Log.Category.System, "Exception During Export to: " + 
				Path.GetFullPath( fileName ) + "\n" + x.Message );
		}
		finally
		{
			// verify we opened a file in the first place, then ensure it is closed
			if( fileStream != null )
			{
				fileStream.Close( );
			}
		}
	}
	
	private void InitializeStyles( )
	{
		// title bar styling
		titleBarStyle = new GUIStyle( );
		titleBarStyle.padding = new RectOffset( 4, 4, 8, 0 );

		// build a new log style for reuse
		logStyle = new GUIStyle( );
		logStyle.alignment = TextAnchor.UpperLeft;
		
		// setup the message style
		messageStyle = new GUIStyle( "label" );
		messageStyle.alignment = TextAnchor.UpperLeft;
		messageStyle.wordWrap = false;
		messageStyle.padding.top = 0;
		
		// setup the box style for unselected rows in log viewer
		boxStyle = new GUIStyle( "label" );
		boxStyle.alignment = TextAnchor.UpperLeft;
		boxStyle.padding.top = 0;
		
		// setup a right aligned numba style!
		numberStyle = new GUIStyle( "label" );
		numberStyle.alignment = TextAnchor.UpperRight;
		numberStyle.padding.right = 12;
		numberStyle.padding.top = 0;

		// setup the box style for unselected rows in log viewer
		boxStyleSelected = new GUIStyle( "label" );
		boxStyleSelected.alignment = TextAnchor.UpperLeft;
		boxStyleSelected.padding.top = 0;
		boxStyleSelected.fontStyle = FontStyle.Bold;
		
		// setup a right aligned numba style!
		numberStyleSelected = new GUIStyle( "label" );
		numberStyleSelected.alignment = TextAnchor.UpperRight;
		numberStyleSelected.padding.right = 12;
		numberStyleSelected.padding.top = 0;
		numberStyleSelected.fontStyle = FontStyle.Bold;
		
		// selection style		
		selectedStyle = new GUIStyle( );
		selectedStyle.alignment = TextAnchor.UpperLeft;
		selectedStyle.normal.background = logProxy.selectedBackground;
			
		// setup the details scrollable region 
		messageDetailStyle = new GUIStyle( "label" );
		messageDetailStyle.wordWrap = true;
		messageDetailStyle.alignment = TextAnchor.UpperLeft;
		messageDetailStyle.padding.right = 12;
		messageDetailStyle.padding.top = 0;

		messageDetailStyleSelected = new GUIStyle( "label" );
		messageDetailStyleSelected.wordWrap = true;
		messageDetailStyleSelected.alignment = TextAnchor.UpperLeft;
		messageDetailStyleSelected.padding.right = 12;
		messageDetailStyleSelected.padding.top = 0;
		messageDetailStyleSelected.fontStyle = FontStyle.Bold;
		
		// setup vertical scrollbar style
		defaultScrollStyle = new GUIStyle( GUI.skin.verticalScrollbar );
		pausedScrollStyle = new GUIStyle( GUI.skin.verticalScrollbar );
		pausedScrollStyle.normal.background = logProxy.pauseBackground;
		
		// set flag so we do not perform this initialization again
		stylesInitialized = true;
	}
	
	private void InitializeColors( )
	{
		// check if we are setting for pro or basic unity3d
		if( EditorGUIUtility.isProSkin )
		{
			// set default text color
			defaultTextColour = new StateColour( );
			defaultTextColour.normal = new Color( 0.85f, 0.85f, 0.85f, 1f );
			defaultTextColour.selected = new Color( 0.85f, 0.85f, 0.85f, 1f );

			// set text colours
			levelColours[ Log.Level.Critical ] = new StateColour( );
			levelColours[ Log.Level.Critical ].normal = new Color( 0.9f, 0.47f, 0.47f, 1f );
			levelColours[ Log.Level.Critical ].selected = new Color( 0.95f, 0.75f, 0.75f, 1f );

			levelColours[ Log.Level.Important ] = new StateColour( );
			levelColours[ Log.Level.Important ].normal = new Color( 0.85f, 0.75f, 0.15f, 1f );
			levelColours[ Log.Level.Important ].selected = new Color( 0.85f, 0.75f, 0.15f, 1f );
		}
		else
		{
			// set default background color
			defaultTextColour = new StateColour( );
			defaultTextColour.normal = Color.black;
			defaultTextColour.selected = Color.white;			
			
			// set text colours
			levelColours[ Log.Level.Critical ] = new StateColour( );
			levelColours[ Log.Level.Critical ].normal = new Color( 0.55f, 0.1f, 0.1f, 1f );
			levelColours[ Log.Level.Critical ].selected = new Color( 0.95f, 0.75f, 0.75f, 1f );

			levelColours[ Log.Level.Important ] = new StateColour( );
			levelColours[ Log.Level.Important ].normal = new Color( 0.55f, 0.42f, 0.1f, 1f );
			levelColours[ Log.Level.Important ].selected = Color.yellow;
		}		
	}
	
	private void HandleKeys( )
	{
		if( Event.current.type == EventType.ScrollWheel )
		{
			if( Event.current.delta.y > 0f )
			{
				scrollPosition = Mathf.Clamp( scrollPosition + 1, 0, logSize );
				isDirty = true;
			}
			else
			if( Event.current.delta.y < 0f )
			{
				scrollPosition = Mathf.Clamp( scrollPosition - 1, 0, logSize );
				isDirty = true;
			}
		}
		else
		if( Event.current.type == EventType.KeyUp )
		{
			if( Event.current.keyCode == KeyCode.C && Event.current.control )
			{
				EditorGUIUtility.systemCopyBuffer = activeMessage.message;
			}
			else
			if( Event.current.keyCode == KeyCode.Pause )
			{
				pauseFrame = logMessages;
			}
			else
			if( Event.current.keyCode == KeyCode.DownArrow )
			{
				scrollPosition = Mathf.Clamp( scrollPosition + 1, 0, logSize );
				isDirty = true;
			}
			else
			if( Event.current.keyCode == KeyCode.PageDown )
			{
				scrollPosition = Mathf.Clamp( scrollPosition + 10, 0, logSize );
				isDirty = true;
			}
			else
			if( Event.current.keyCode == KeyCode.UpArrow )
			{
				scrollPosition = Mathf.Clamp( scrollPosition - 1, 0, logSize );
				isDirty = true;
			}
			else
			if( Event.current.keyCode == KeyCode.PageUp )
			{
				scrollPosition = Mathf.Clamp( scrollPosition - 10, 0, logSize );
				isDirty = true;
			}
			else
			if( Event.current.keyCode == KeyCode.Home )
			{
				scrollPosition = 0;
				isDirty = true;
			}
			else
			if( Event.current.keyCode == KeyCode.End )
			{
				scrollPosition = logSize;
				isDirty = true;
			}
		}
	}
	
	public void ShowWatches( bool show )
	{
		if( show )
		{
			logWatch = ( LogWatcher )EditorWindow.GetWindow( typeof( LogWatcher ) );
			logWatch.title = "Log Watches";
		}
		else
		{
			logWatch.Close( );
			logWatch = null;
		}
	}

	private void ToggleWatchWindow( )
	{
		ShowWatches( logWatch == null );
	}
	
    public void HandleLog( string logString, string stackTrace, LogType type ) 
	{
		// check if we want to intercept log messages or just let them go to console only		
		if( !Log.Instance.IsInterceptOn( ) || Log.Instance.IsConsoleOn( ) )
		{
			return;
		}
		
		// determine the level mapping from a console event to a logging event
		Log.Level level;
		if( type == LogType.Error || type == LogType.Exception )
		{
			level = Log.Level.Critical;
		}
		else 
		if( type == LogType.Warning || type == LogType.Assert )
		{
			level = Log.Level.Important;
		}
		else
		{
			level = Log.Level.Normal;
		}
		
		// log the details, include stack trace
		Log.Record( Log.Category.Console, level, logString, stackTrace.Trim( '\n' ) );	
    }		
	
	private void InterceptConsole( )
	{
		Log.Instance.SetIntercept( !Log.Instance.IsInterceptOn( ) );
		if( Log.Instance.IsInterceptOn( ) )
		{
			Log.Instance.SetConsole( false );
		}

		SaveSettings( );
	}
	
	private void RedirectToConsole( )
	{
		Log.Instance.SetConsole( !Log.Instance.IsConsoleOn( ) );
		if( Log.Instance.IsConsoleOn( ) )
		{
			Log.Instance.SetIntercept( false );
		}
		
		SaveSettings( );
	}

	private void SaveSettings( )
	{
#if UNITY_3
		// write out all options to options file
		File.WriteAllLines( FILE_NAME, new string[ ] {
			Log.Instance.IsConsoleOn( ).ToString( ),
			Log.Instance.IsInterceptOn( ).ToString( )
		} );
#else
		// write out all options to options file, for 4.x+
		StreamWriter writer = null;
		
		try
		{
			// create a new target options file and spit out console + intercept options
			FileStream fileStream = System.IO.File.Create( FILE_NAME );
			writer = new StreamWriter( fileStream );
			writer.WriteLine( Log.Instance.IsConsoleOn( ).ToString( ) );
			writer.WriteLine( Log.Instance.IsInterceptOn( ).ToString( ) );
		}
		finally
		{
			if( writer != null )
			{
				writer.Close( );
			}
		}
#endif
	}
	private void LoadSettings( )
	{
		// only attempt a load if we have settings persisted already
		if( File.Exists( FILE_NAME ) )
		{
			// read in all options, just assume ordering as this file should not be modified by user
			string [ ] options = File.ReadAllLines( FILE_NAME );
			Log.Instance.SetConsole( bool.Parse( options[ 0 ] ) );
			Log.Instance.SetIntercept( bool.Parse( options[ 1 ] ) );
		}
	}
	
	private StateColour GetStateColorByLevel( Log.Level level )
	{
		if( levelColours.ContainsKey( level ) )
		{
			return levelColours[ level ];
		}
		else
		{
			return defaultTextColour;
		}
	}
}