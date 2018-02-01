/* Written by Kaz Crowe */
/* SimpleHealthBarEditor.cs */
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

[CanEditMultipleObjects]
[CustomEditor( typeof( SimpleHealthBar ) )]
public class SimpleHealthBarEditor : Editor
{
	SimpleHealthBar targ;

	float testValue = 100.0f;

	SerializedProperty colorMode, statusImage;
	SerializedProperty statusColor, statusGradient;

	SerializedProperty displayText, statusText;
	SerializedProperty additionalText;

	SerializedProperty fillConstraint;
	SerializedProperty fillConstraintMin, fillConstraintMax;

	SerializedProperty statusName;

	Color statusTextColor = Color.white;
	AnimBool ImageColorWarning, NameUnassigned;
	AnimBool ExampleCode;
	AnimBool DuplicateStatusName, FillConstraintOptions;
	AnimBool ImageWarning, DisplayTextOptions;
	AnimBool StatusImageAssigned, StatusImageUnassigned;

	enum FunctionList
	{
		UpdateBar,
		UpdateColor,
		UpdateTextColor
	}
	FunctionList functionList;
	string exampleCode;


	void OnEnable ()
	{
		// Store the references to all variables.
		StoreReferences();

		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;

		if( targ != null && targ.barImage != null )
		{
			float tempFloat = 0.0f;
			if( targ.fillConstraint == true )
				tempFloat = ( targ.barImage.fillAmount - targ.fillConstraintMin ) / ( targ.fillConstraintMax - targ.fillConstraintMin );
			else
				tempFloat = targ.barImage.fillAmount;

			testValue = tempFloat * 100.0f;
		}
	}

	void OnDisable ()
	{
		// Remove the UndoRedoCallback from the Undo event.
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}
	
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		
		EditorGUILayout.BeginVertical( "Box" );

		// ----- < BAR NAME > ----- //
		if( statusName.stringValue == string.Empty && Event.current.type == EventType.Repaint )
		{
			GUIStyle style = new GUIStyle( GUI.skin.textField );
			style.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 0.75f );
			EditorGUILayout.TextField( new GUIContent( "Bar Name", "The unique name to be used in reference to this bar." ), "Bar Name", style );
		}
		else
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( statusName, new GUIContent( "Bar Name", "The unique name to be used in reference to this bar." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				DuplicateStatusName.target = GetDuplicateBarName();
				NameUnassigned.target = targ.barName == string.Empty;
				ExampleCode.target = targ.barName != string.Empty && !GetDuplicateBarName();
				GenerateExampleCode();
			}
		}
		// ----- < END BAR NAME > ----- //

		// ----- < NAME ERRORS > ----- //
		if( EditorGUILayout.BeginFadeGroup( DuplicateStatusName.faded ) )
			EditorGUILayout.HelpBox( "The bar name \"" + targ.barName + "\" is already being used in this scene. Please make each Simple Health Bar has a unique name.", MessageType.Warning );
		EditorGUILayout.EndFadeGroup();
		
		if( EditorGUILayout.BeginFadeGroup( ExampleCode.faded ) )
		{
			GUILayout.Space( 1 );
			EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );
			EditorGUI.BeginChangeCheck();
			functionList = ( FunctionList )EditorGUILayout.EnumPopup( "Function", functionList );
			if( EditorGUI.EndChangeCheck() )
				GenerateExampleCode();

			EditorGUILayout.TextField( exampleCode );

			GUILayout.Space( 1 );
		}
		EditorGUILayout.EndFadeGroup();
		// ----- < END NAME ERRORS > ----- //

		EditorGUILayout.EndVertical();

		// ----- < BAR IMAGE > ----- //
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( statusImage, new GUIContent( "Image", "The image component to be used for this bar." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			if( targ.barImage != null && targ.barImage.type != Image.Type.Filled )
			{
				targ.barImage.type = Image.Type.Filled;
				targ.barImage.fillMethod = Image.FillMethod.Horizontal;
				EditorUtility.SetDirty( targ.barImage );
			}
			if( targ.barImage != null )
			{
				statusColor.colorValue = targ.barImage.color;
				serializedObject.ApplyModifiedProperties();
			}
			targ.UpdateBar( testValue, 100.0f );

			ImageWarning.target = GetBarImageWarning();
			StatusImageAssigned.target = GetImageAssigned();
			StatusImageUnassigned.target = GetImageUnassigned();
		}

		if( EditorGUILayout.BeginFadeGroup( StatusImageUnassigned.faded ) )
		{
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.HelpBox( "Image is unassigned.", MessageType.Warning );
			if( GUILayout.Button( "Find", EditorStyles.miniButton ) )
			{
				statusImage.objectReferenceValue = targ.GetComponent<Image>();
				serializedObject.ApplyModifiedProperties();
				if( targ.barImage != null )
				{
					targ.barImage.type = Image.Type.Filled;
					targ.barImage.fillMethod = Image.FillMethod.Horizontal;
					EditorUtility.SetDirty( targ.barImage );

					statusColor.colorValue = targ.barImage.color;
					serializedObject.ApplyModifiedProperties();
				}

				ImageWarning.target = GetBarImageWarning();
				StatusImageAssigned.target = GetImageAssigned();
				StatusImageUnassigned.target = GetImageUnassigned();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndFadeGroup();
		// ----- < END BAR IMAGE > ----- //

		if( EditorGUILayout.BeginFadeGroup( StatusImageAssigned.faded ) )
		{
			// ----- < BAR IMAGE ERROR > ----- //
			if( EditorGUILayout.BeginFadeGroup( ImageWarning.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "Invalid Image Type: " + targ.barImage.type.ToString(), MessageType.Warning );
				if( GUILayout.Button( "Fix", EditorStyles.miniButton ) )
				{
					targ.barImage.type = Image.Type.Filled;
					EditorUtility.SetDirty( targ.barImage );

					ImageWarning.target = GetBarImageWarning();
				}
				EditorGUILayout.EndVertical();
			}
			if( StatusImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END BAR IMAGE ERROR > ----- //

			// ----- < BAR COLORS > ----- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( colorMode, new GUIContent( "Color Mode", "The mode in which to display the color to the image component." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				UpdateColor();
				ImageColorWarning.target = GetColorWarning();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUI.indentLevel = 1;
			if( targ.colorMode == SimpleHealthBar.ColorMode.Single )
				EditorGUILayout.PropertyField( statusColor, new GUIContent( "Image Color", "The color of this image." ) );
			else
				EditorGUILayout.PropertyField( statusGradient, new GUIContent( "Image Gradient", "The color gradient of this image." ) );
			EditorGUI.indentLevel = 0;
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				UpdateColor();
				ImageColorWarning.target = GetColorWarning();
			}

			if( EditorGUILayout.BeginFadeGroup( ImageColorWarning.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "Image color has been modified incorrectly.", MessageType.Warning );
				EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( "Update Image", EditorStyles.miniButtonLeft ) )
				{
					targ.barImage.color = statusColor.colorValue;
					EditorUtility.SetDirty( targ.barImage );
					ImageColorWarning.target = GetColorWarning();
				}
				if( GUILayout.Button( "Update Script", EditorStyles.miniButtonRight ) )
				{
					statusColor.colorValue = targ.barImage.color;
					serializedObject.ApplyModifiedProperties();
					ImageColorWarning.target = GetColorWarning();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			if( StatusImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END BAR COLORS > ----- //

			EditorGUILayout.Space();

			// ------- < TEXT OPTIONS > ------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( displayText, new GUIContent( "Display Text", "Determines how this bar will display text to the user." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				DisplayTextOptions.target = targ.displayText != SimpleHealthBar.DisplayText.Disabled;

				targ.UpdateBar( testValue, 100.0f );
				if( statusText.objectReferenceValue != null )
					EditorUtility.SetDirty( targ.barText );
			}

			if( EditorGUILayout.BeginFadeGroup( DisplayTextOptions.faded ) )
			{
				EditorGUI.indentLevel = 1;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( statusText, new GUIContent( "Text", "The Text component to be used for the status text." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateTextColor( statusTextColor );
					targ.UpdateBar( testValue, 100.0f );
					if( statusText.objectReferenceValue != null )
						EditorUtility.SetDirty( targ.barText );
				}

				EditorGUI.BeginChangeCheck();
				statusTextColor = EditorGUILayout.ColorField( new GUIContent( "Text Color", "The color of the Text component." ), statusTextColor );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateTextColor( statusTextColor );
					if( statusText.objectReferenceValue != null )
						EditorUtility.SetDirty( targ.barText );
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( additionalText, new GUIContent( "Additional Text", "Additional text to be displayed before the current information." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateBar( testValue, 100.0f );
					if( statusText.objectReferenceValue != null )
						EditorUtility.SetDirty( targ.barText );
				}

				EditorGUI.indentLevel = 2;
				switch( targ.displayText )
				{
					case SimpleHealthBar.DisplayText.Percentage:
					{
						EditorGUILayout.LabelField( "Text Preview: " + targ.additionalText + testValue + "%" );
					}
					break;
					case SimpleHealthBar.DisplayText.CurrentValue:
					{
						EditorGUILayout.LabelField( "Text Preview: " + targ.additionalText + testValue );
					}
					break;
					case SimpleHealthBar.DisplayText.CurrentAndMaxValues:
					{
						EditorGUILayout.LabelField( "Text Preview: " + targ.additionalText + testValue + " / 100" );
					}
					break;
					default:
					{
						EditorGUILayout.LabelField( "Text Preview: Default" );
					}
					break;
				}
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StatusImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END TEXT OPTIONS > ----- //

			// ----- < FILL CONSTRAINT > ----- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( fillConstraint, new GUIContent( "Fill Constraint", "Determines whether or not the image fill should be constrained." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				FillConstraintOptions.target = targ.fillConstraint;
			}

			if( EditorGUILayout.BeginFadeGroup( FillConstraintOptions.faded ) )
			{
				EditorGUI.indentLevel = 1;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider( fillConstraintMin, 0.0f, targ.fillConstraintMax, new GUIContent( "Fill Minimum", "The minimum fill amount." ) );
				EditorGUILayout.Slider( fillConstraintMax, targ.fillConstraintMin, 1.0f, new GUIContent( "Fill Maximum", "The maximum fill amount." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					if( targ.barImage != null )
					{
						targ.barImage.enabled = false;
						targ.UpdateBar( testValue, 100.0f );
						targ.barImage.enabled = true;
					}
				}

				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( StatusImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// --- < END FILL CONSTRAINT > --- //

			// ----- < TEST VALUE > ----- //
			EditorGUI.BeginChangeCheck();
			testValue = EditorGUILayout.Slider( new GUIContent( "Test Value" ), testValue, 0.0f, 100.0f );
			if( EditorGUI.EndChangeCheck() )
			{
				if( targ.barImage != null )
				{
					targ.barImage.enabled = false;
					targ.UpdateBar( testValue, 100.0f );
					targ.barImage.enabled = true;

					EditorUtility.SetDirty( targ.barImage );
				}
			}
			// ----- < END TEST VALUE > ----- //
		}
		EditorGUILayout.EndFadeGroup();

		Repaint();
	}

	void StoreReferences ()
	{
		// Assign the current target.
		targ = ( SimpleHealthBar ) target;

		colorMode = serializedObject.FindProperty( "colorMode" );
		statusImage = serializedObject.FindProperty( "barImage" );
		statusColor = serializedObject.FindProperty( "barColor" );
		statusGradient = serializedObject.FindProperty( "barGradient" );

		displayText = serializedObject.FindProperty( "displayText" );
		statusText = serializedObject.FindProperty( "barText" );
		additionalText = serializedObject.FindProperty( "additionalText" );

		fillConstraint = serializedObject.FindProperty( "fillConstraint" );
		fillConstraintMin = serializedObject.FindProperty( "fillConstraintMin" );
		fillConstraintMax = serializedObject.FindProperty( "fillConstraintMax" );

		statusName = serializedObject.FindProperty( "barName" );

		ImageWarning = new AnimBool( GetBarImageWarning() );
		DisplayTextOptions = new AnimBool( targ.displayText != SimpleHealthBar.DisplayText.Disabled );
		ImageColorWarning = new AnimBool( GetColorWarning() );
		DuplicateStatusName = new AnimBool( GetDuplicateBarName() );
		NameUnassigned = new AnimBool( targ.barName == string.Empty );
		FillConstraintOptions = new AnimBool( targ.fillConstraint );

		ExampleCode = new AnimBool( targ.barName != string.Empty && !GetDuplicateBarName() );
		exampleCode = "SimpleHealthBar.UpdateBar( \"" + targ.barName + "\", currentValue, maxValue );";

		StatusImageAssigned = new AnimBool( GetImageAssigned() );
		StatusImageUnassigned = new AnimBool( GetImageUnassigned() );
	}

	void UpdateColor ()
	{
		// If the image component is null, then return.
		if( targ.barImage == null )
			return;

		// Switch statement for the color mode option. Each case handles the color according to the option.
		switch( targ.colorMode )
		{
			case SimpleHealthBar.ColorMode.Single:
			{
				targ.barImage.color = targ.barColor;
			} break;
			case SimpleHealthBar.ColorMode.Gradient:
			{
				targ.barImage.color = targ.barGradient.Evaluate( targ.GetCurrentFraction );
			} break;
		}
		EditorUtility.SetDirty( targ.barImage );
	}

	bool GetImageAssigned ()
	{
		if( targ.barImage != null )
			return true;
		return false;
	}

	bool GetImageUnassigned ()
	{
		if( targ.barImage == null )
			return true;
		return false;
	}

	bool GetBarImageWarning ()
	{
		if( targ.barImage != null && targ.barImage.type != Image.Type.Filled )
			return true;
		return false;
	}

	bool GetColorWarning ()
	{
		if( Application.isPlaying == true )
			return false;

		if( targ.barImage == null )
			return false;

		if( targ.colorMode == SimpleHealthBar.ColorMode.Single && targ.barImage.color != targ.barColor )
			return true;

		return false;
	}

	bool GetDuplicateBarName ()
	{
		SimpleHealthBar[] allHealthBars = FindObjectsOfType<SimpleHealthBar>();
		for( int i = 0; i < allHealthBars.Length; i++ )
		{
			if( targ != allHealthBars[ i ] && targ.barName == allHealthBars[ i ].barName && allHealthBars[ i ].barName != string.Empty )
				return true;
		}

		return false;
	}

	void GenerateExampleCode ()
	{
		switch( functionList )
		{
			default:
			case FunctionList.UpdateBar:
			{
				exampleCode = "SimpleHealthBar.UpdateBar( \"" + targ.barName + "\", currentValue, maxValue );";
			}
			break;
			case FunctionList.UpdateColor:
			{
				exampleCode = "SimpleHealthBar.UpdateStatusColor( \"" + targ.barName + "\", newColor );";
			}
			break;
			case FunctionList.UpdateTextColor:
			{
				exampleCode = "SimpleHealthBar.UpdateTextColor( \"" + targ.barName + "\", newTextColor );";
			}
			break;
		}
	}
}