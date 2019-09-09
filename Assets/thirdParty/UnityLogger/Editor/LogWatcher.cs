#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
	#define UNITY_4
#endif

#if UNITY_3_5 || UNITY_3_6 || UNITY_3_7 || UNITY_3_8 || UNITY_3_9
	#define UNITY_3
#endif

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[InitializeOnLoad]
public class LogWatcher : EditorWindow 
{
	[SerializeField] private LogProxy logProxy;
	
	private Vector2 scrollView;
	private GUIStyle backgroundStyle;
	private bool stylesInitialized;
	
    static void Init () 
	{
		// build the log viewer window and set title, also init logwatch to null for first run
        LogWatcher window = ( LogWatcher )EditorWindow.GetWindow( typeof( LogWatcher ) );
		window.title = "Log Watcher";
	}
	
    LogWatcher( )
    {
        EditorApplication.update += Update;
		backgroundStyle = new GUIStyle( );
    }
	
	void OnEnable( )
	{
		logProxy = new LogProxy( );
	}
	
    void Update( )
    {
        Repaint( );
    }
	
	void OnInspectorUpdate( )
	{
		Repaint( );
	}
	
	void OnGUI( )
	{
		scrollView = GUILayout.BeginScrollView( scrollView );
		
		int row = 0;
		foreach( string watch in Log.Instance.GetAllWatches( ).Keys )
		{
			// alternate banding for easier reading, use log viewers current settings
			if( row % 2 == 0 )
			{
				backgroundStyle.normal.background = logProxy.evenBackground;
			}
			else
			{
				backgroundStyle.normal.background = logProxy.oddBackground;
			}
			
			GUILayout.BeginHorizontal( backgroundStyle );
			GUILayout.Label( watch, GUILayout.Width( 172f ) );
			GUILayout.Label( Log.Instance.GetAllWatches( )[ watch ], GUILayout.ExpandWidth( true ) );
			GUILayout.EndHorizontal( );
			
			row ++;
		}
		
		GUILayout.EndScrollView( );
	}
}
