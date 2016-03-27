﻿using System;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace Vixen.Data.Value
{
	public struct DiscreteValue : IIntentDataType
	{
		private readonly double _hue;
		private readonly double _saturation;
		private readonly double _value;

		public DiscreteValue(Color color)
		{
			HSV.FromRGB(color, out _hue, out _saturation, out _value);
		}

		public DiscreteValue(Color color, double intensity)
			: this(color)
		{
			_value = intensity;
		}

		public DiscreteValue(double h, double s, double i)
		{
			_hue = XYZ.ClipValue(h, 0.0, 1.0);
			_saturation = XYZ.ClipValue(s, 0.0, 1.0);
			_value = XYZ.ClipValue(i, 0.0, 1.0);
		}

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public double Hue
		{
			get { return _hue; }
		}

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public double Saturation
		{
			get { return _saturation; }
		}

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public double Intensity
		{
			get { return _value; }
		}

		/// <summary>
		/// The lighting value as a color with the intensity value applied. Results in an opaque color ranging from black
		/// (0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColor
		{
			get { return HSV.ToRGB(_hue, _saturation, _value).ToArgb(); }
		}

		/// <summary>
		/// Gets the lighting value as a color with the intensity value applied to the alpha channel.
		/// Results in a color of variable transparancy.
		/// </summary>
		public Color FullColorWithAplha
		{
			get
			{
				Color c = FullColor;
				return Color.FromArgb((int)(Intensity * byte.MaxValue), c.R, c.G, c.B);
			}
		}

		/// <summary>
		/// This is the full color portion of the lighting value with the Inesity applied only to the Alpha portion of 
		/// the color. See VIX-430
		/// </summary>
		public Color TrueFullColorWithAlpha
		{
			get
			{
				return Color.FromArgb((int)(Intensity * byte.MaxValue), HueSaturationOnlyColor);
			}
		}

		/// <summary>
		/// The 'color' portion of the lighting value; ie. only the Hue and Saturation.
		/// This is equivalent to the full color that would have an intensity of 1 (or 100%).
		/// </summary>
		public Color HueSaturationOnlyColor
		{
			get
			{
				Color rv = HSV.ToRGB(_hue, _saturation, 1).ToArgb();
				return rv;
			}
		}
	}
}
