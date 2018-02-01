/* Written by Kaz Crowe */
/* SimpleHealthBar.cs */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SimpleHealthBar : MonoBehaviour
{	
	// COLOR OPTIONS //
	public enum ColorMode
	{
		Single,
		Gradient
	}
	public ColorMode colorMode;
	public Image barImage;
	public Color barColor = Color.white;
	public Gradient barGradient = new Gradient();

	// TEXT OPTIONS //
	public enum DisplayText
	{
		Disabled,
		Percentage,
		CurrentValue,
		CurrentAndMaxValues
	}
	public DisplayText displayText;
	public Text barText;
	public string additionalText = string.Empty;

	// FILL CONSTRAINT //
	public bool fillConstraint = false;
	public float fillConstraintMin = 0.0f;
	public float fillConstraintMax = 1.0f;

	// PRIVATE VARIABLES AND GET FUNCTIONS //
	float currentFraction = 1.0f;
	public float GetCurrentFraction
	{
		get
		{
			return currentFraction;
		}
	}

	float _maxValue = 0.0f;

	// ----- < SCRIPT REFERENCE > ----- //
	static Dictionary<string, SimpleHealthBar> SimpleHealthBars = new Dictionary<string, SimpleHealthBar>();
	public string barName = "";


	void Awake ()
	{
		// If the application is not playing, then return.
		if( Application.isPlaying == false )
			return;

		// If the bar name is assigned, then register this to the dictionary.
		if( barName != string.Empty )
		{
			if( SimpleHealthBars.ContainsKey( barName ) )
				SimpleHealthBars.Remove( barName );

			SimpleHealthBars.Add( barName, this );
		}
	}

	/* PUBLIC FUNCTIONS */
	/// <summary>
	/// Updates the bar with the current and max values.
	/// </summary>
	/// <param name="currentValue">The current value of the bar.</param>
	/// <param name="maxValue">The maximum value of the bar.</param>
	public void UpdateBar ( float currentValue, float maxValue )
	{
		// If the bar image is left unassigned, then return.
		if( barImage == null )
			return;
			
		// Fix the value to be a percentage.
		currentFraction = currentValue / maxValue;

		// If the value is greater than 1 or less than 0, then fix the values to being min/max.
		if( currentFraction < 0 || currentFraction > 1 )
			currentFraction = currentFraction < 0 ? 0 : 1;

		// Store the values so that other functions used can reference the maxValue.
		_maxValue = maxValue;

		// Then just apply the target fill according to the users options.
		barImage.fillAmount = fillConstraint == true ? Mathf.Lerp( fillConstraintMin, fillConstraintMax, currentFraction ) : currentFraction;

		// If the color mode is set to Gradient, then apply the current gradient color.
		if( colorMode == ColorMode.Gradient )
			barImage.color = barGradient.Evaluate( currentFraction );

		// If the user does not want text to be displayed, or the text component is null, then return.
		if( displayText != DisplayText.Disabled && barText != null )
		{
			// Switch statement for the displayText option. Each option will display the correct text for the set option.
			switch( displayText )
			{
				case DisplayText.Percentage:
				{
					barText.text = additionalText + ( currentFraction * 100 ).ToString( "F0" ) + "%";
				}
				break;
				case DisplayText.CurrentValue:
				{
					barText.text = additionalText + ( currentFraction * _maxValue ).ToString( "F0" );
				}
				break;
				case DisplayText.CurrentAndMaxValues:
				{
					barText.text = additionalText + ( currentFraction * _maxValue ).ToString( "F0" ) + " / " + _maxValue.ToString();
				}
				break;
			}
		}
	}

	/// <summary>
	/// Updates the color of the bar with the target color.
	/// </summary>
	/// <param name="targetColor">The target color to apply to the bar.</param>
	public void UpdateColor ( Color targetColor )
	{
		// If the color is not set to single, then return.
		if( colorMode != ColorMode.Single || barImage == null )
			return;

		// Set the color to the new target color and apply it to the bar.
		barColor = targetColor;
		barImage.color = barColor;
	}

	/// <summary>
	/// Updates the gradient of the bar with the target gradient.
	/// </summary>
	/// <param name="targetColor">The target color to apply to the bar.</param>
	public void UpdateColor ( Gradient targetGradient )
	{
		// If the color is not set to single, then return.
		if( colorMode != ColorMode.Gradient || barImage == null )
			return;

		// Set the gradient to the new target gradient and apply it to the bar.
		barGradient = targetGradient;
		barImage.color = barColor;
	}

	/// <summary>
	/// Updates the color of the text associated with the bar.
	/// </summary>
	/// <param name="targetColor">The target color to apply to the text component.</param>
	public void UpdateTextColor ( Color targetColor )
	{
		// If the user is not wanting the text to be displayed, or the text component is not assigned, then return.
		if( displayText == DisplayText.Disabled || barText == null)
			return;

		// Set the text color to the new target color and apply it to the text component.
		barText.color = targetColor;
	}

	/* PUBLIC STATIC FUNCTIONS */
	/// <summary>
	/// Updates the targeted bar with the current and max values.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	/// <param name="currentValue">The current value of the bar.</param>
	/// <param name="maxValue">The maximum value of the bar.</param>
	public static void UpdateBar ( string barName, float currentValue, float maxValue )
	{
		if( !SimpleHealthBarRegistered( barName ) )
			return;

		SimpleHealthBars[ barName ].UpdateBar( currentValue, maxValue );
	}

	/// <summary>
	/// Updates the targeted image with the target color.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	/// <param name="targetColor">The targeted color to apply to the image.</param>
	public static void UpdateColor ( string barName, Color targetColor )
	{
		if( !SimpleHealthBarRegistered( barName ) )
			return;

		SimpleHealthBars[ barName ].UpdateColor( targetColor );
	}

	/// <summary>
	/// Updates the targeted image with the target gradient.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	/// <param name="targetGradient">The targeted gradient to apply to the image.</param>
	public static void UpdateColor ( string barName, Gradient targetGradient )
	{
		if( !SimpleHealthBarRegistered( barName ) )
			return;

		SimpleHealthBars[ barName ].UpdateColor( targetGradient );
	}

	/// <summary>
	/// Updates the targeted text with the target color.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	/// <param name="targetColor">The targeted color to apply to the text component.</param>
	public static void UpdateTextColor ( string barName, Color targetColor )
	{
		if( !SimpleHealthBarRegistered( barName ) )
			return;

		SimpleHealthBars[ barName ].UpdateTextColor( targetColor );
	}

	/// <summary>
	/// Returns the Simple Health Bar that has been registered with the targeted name.
	/// </summary>
	/// <param name="barName">The name of the targeted Simple Health Bar.</param>
	public static SimpleHealthBar GetSimpleHealthBar ( string barName )
	{
		if( !SimpleHealthBarRegistered( barName ) )
			return null;

		return SimpleHealthBars[ barName ];
	}

	/* PRIVATE FUNCTIONS */
	static bool SimpleHealthBarRegistered ( string barName )
	{
		if( SimpleHealthBars.ContainsKey( barName ) )
			return true;

		Debug.LogWarning( "Simple Health Bar - No Simple Health Bar has been registered with the name: " + barName + "." );
		return false;
	}
}