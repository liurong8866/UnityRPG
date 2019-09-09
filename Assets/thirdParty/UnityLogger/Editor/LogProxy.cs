using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LogProxy
{
	// the background texture2d's used for colour scheme
	[SerializeField] public Texture2D evenBackground;	
	[SerializeField] public Texture2D oddBackground;
	[SerializeField] public Texture2D pauseBackground;
	[SerializeField] public Texture2D selectedBackground;
	
	public LogProxy( )
	{
		// check if we are setting for pro or basic unity3d
		if( EditorGUIUtility.isProSkin )
		{
			// apparently unity3d 3.x vs 4.x adjusts colours differently, treat 3.x differently
			evenBackground = ( Texture2D )Resources.Load( "texture_evenrow_dark_3" );
			oddBackground = ( Texture2D )Resources.Load( "texture_oddrow_dark_3" );
			selectedBackground = ( Texture2D )Resources.Load( "texture_selected_dark_3" );

			/*
			if( Application.unityVersion.StartsWith( "3" ) ) 
			{

			}
			else
			{
				evenBackground = ( Texture2D )Resources.Load( "texture_evenrow_dark" );
				oddBackground = ( Texture2D )Resources.Load( "texture_oddrow_dark" );
				selectedBackground = ( Texture2D )Resources.Load( "texture_selected_dark" );
			}
			*/
		}
		else
		{
			// apparently unity3d 3.x vs 4.x adjusts colours differently, treat 3.x differently
			if( Application.unityVersion.StartsWith( "3" ) ) 
			{
				evenBackground = ( Texture2D )Resources.Load( "texture_evenrow_light_3" );
				oddBackground = ( Texture2D )Resources.Load( "texture_oddrow_light_3" );
				selectedBackground = ( Texture2D )Resources.Load( "texture_selected_light_3" );
			}
			else
			{
				evenBackground = ( Texture2D )Resources.Load( "texture_evenrow_light" );
				oddBackground = ( Texture2D )Resources.Load( "texture_oddrow_light" );
				selectedBackground = ( Texture2D )Resources.Load( "texture_selected_light" );
			}
		}

		pauseBackground = ( Texture2D )Resources.Load( "texture_pause" );
	}
}

/*

	private void InitializeColors( )
	{
		// check if we are setting for pro or basic unity3d
		if( EditorGUIUtility.isProSkin )
		{
			// set default background color
			defaultTextColour = new Color( 0.85f, 0.85f, 0.85f, 1f );

			// set text colours
			levelColours[ Log.Level.Critical ] = new Color( 0.9f, 0.47f, 0.47f, 1f );
			levelColours[ Log.Level.Important ] = new Color( 0.85f, 0.75f, 0.15f, 1f );
	
			// apparently unity3d 3.x vs 4.x adjusts colours differently, treat 3.x differently
			if( Application.unityVersion.StartsWith( "3" ) ) 
			{
				// set odd and even banding backgrounds
				oddBackground = MakeTexture( new Color( 0.225f, 0.225f, 0.225f, 1f ) );
				evenBackground = MakeTexture( new Color( 0.25f, 0.25f, 0.25f, 1f ) );

				// set a colour for the background when a row is selected
				selectedBackground = MakeTexture( new Color( 0.175f, 0.175f, 0.175f, 1f ) );
			}
			else
			{
				// set odd and even banding backgrounds
				oddBackground = MakeTexture( new Color( 0.525f, 0.525f, 0.525f, 1f ) );
				evenBackground = MakeTexture( new Color( 0.55f, 0.55f, 0.55f, 1f ) );
				
				// set a colour for the background when a row is selected
				selectedBackground = MakeTexture( new Color( 0.475f, 0.475f, 0.475f, 1f ) );
			}
		}
		else
		{
			// set text colours
			defaultTextColour = Color.black;
			levelColours[ Log.Level.Critical ] = new Color( 0.45f, 0.1f, 0.1f, 1f );
			levelColours[ Log.Level.Important ] = new Color( 0.65f, 0.52f, 0.1f, 1f );

			// set odd and even banding backgrounds
			oddBackground = MakeTexture( new Color( 0.95f, 0.95f, 0.95f, 1f ) );
			evenBackground = MakeTexture( new Color( 0.925f, 0.925f, 0.925f, 1f ) );

			// set a colour for the background when a row is selected
			selectedBackground = MakeTexture( Color.white );
		}		
		
		pauseBackground = MakeTexture( Color.red );
	}
*/