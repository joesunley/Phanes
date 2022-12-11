namespace Phanes.Mapper;

public sealed class Font
{
	public string Name { get; set; }
	
	public string FontFamily { get; set; }
	
	public float Size { get; set; } // Size in mm
	
	public float LineSpacing { get; set; }
	public float ParagraphSpacing { get; set; }
	public float CharacterSpacing { get; set; }

	public FontStyle FontStyle { get; set; }

	public Font(string name, string fontFamily, float size, float lineSpacing, float paragraphSpacing, float characterSpacing, FontStyle fontStyle)
	{
		Name = name;
		FontFamily = fontFamily;
		Size = size;
		LineSpacing = lineSpacing;
		ParagraphSpacing = paragraphSpacing;
		CharacterSpacing = characterSpacing;
		FontStyle = fontStyle;
	}

	public static float ConvertPointsToMillimeters(float points)
	{
		return points * 0.352777778f;
	}
	public static float ConvertMillimetersToPoints(float millimeters)
	{
		return millimeters * 2.83464567f;
	}
}

[Flags]
public enum FontStyle : byte
{
	Default = 0,
	Bold = 1,
	Italic = 2,
	Underline = 4,
	Strikeout = 8,
}