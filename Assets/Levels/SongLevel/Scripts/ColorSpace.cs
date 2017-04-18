using UnityEngine;
using System.Collections;

public static class ColorSpace {
	public struct Rgb
	{
		public double r;       // percent
		public double g;       // percent
		public double b;       // percent

		public Rgb(double r, double g, double b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}
	}
	public struct Hsv
	{
		public double h;       // angle in degrees
		public double s;       // percent
		public double v;       // percent

		public Hsv(double h, double s, double v)
		{
			this.h = h;
			this.s = s;
			this.v = v;
		}
	}
	public static Rgb hsv2rgb(Hsv hsv)
	{
		double hh, p, q, t, ff;
		long i;
		double red, green, blue;
		Rgb rgb;

		if (hsv.s <= 0.0)
		{       // < is bogus, just shuts up warnings
			red = hsv.v;
			green = hsv.v;
			blue = hsv.v;
			rgb = new Rgb(red, green, blue);
			return rgb;
		}
		hh = hsv.h;
		if (hh >= 360.0) hh = 0.0;
		hh /= 60.0;
		i = (long)hh;
		ff = hh - i;
		p = hsv.v * (1.0 - hsv.s);
		q = hsv.v * (1.0 - (hsv.s * ff));
		t = hsv.v * (1.0 - (hsv.s * (1.0 - ff)));

		switch (i)
		{
			case 0:
				red = hsv.v;
				green = t;
				blue = p;
				break;
			case 1:
				red = q;
				green = hsv.v;
				blue = p;
				break;
			case 2:
				red = p;
				green = hsv.v;
				blue = t;
				break;

			case 3:
				red = p;
				green = q;
				blue = hsv.v;
				break;
			case 4:
				red = t;
				green = p;
				blue = hsv.v;
				break;
			case 5:
			default:
				red = hsv.v;
				green = p;
				blue = q;
				break;
		}
		rgb = new Rgb(red, green, blue);
		return rgb;
	}
}
