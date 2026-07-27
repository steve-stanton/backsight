using System.Xml.Serialization; // TODO: switch to json?

namespace Backsight.Model;

/// <summary>
/// Options relating to the line annotation (the display of observed distances and angles).
/// </summary>
public class LineAnnotationStyle
{
    /// <summary>
    /// Scale denominator at which line annotation will be drawn.
    /// </summary>
    double m_Scale;

    /// <summary>
    /// Height of line annotation text, in meters on the ground.
    /// </summary>
    double m_Height;

    /// <summary>
    /// Flags indicating what item(s) appear in line annotations.
    /// </summary>
    LineAnnotationOptions m_Options;

    /// <summary>
    /// Default constructor defines a style indicating that line annotation should
    /// not be displayed (if subsequently turned on, the display scale will default
    /// to 1:2000, and the height of the text will default to 2 meters on the ground).
    /// </summary>
    internal LineAnnotationStyle()
    {
        m_Scale = 2000.0;
        m_Height = 2.0;
        m_Options = 0;
    }

    /// <summary>
    /// Scale denominator at which line annotation will be drawn.
    /// </summary>
    [XmlAttribute]
    public double ShowScale
    {
        get => m_Scale;
        set => m_Scale = value;
    }

    /// <summary>
    /// Height of line annotation text, in meters on the ground.
    /// </summary>
    [XmlAttribute]
    public double Height
    {
        get => m_Height;
        set => m_Height = value;
    }

    /// <summary>
    /// Should the adjusted length of lines be displayed?
    /// (the units used for the display is determined via another display preference)
    /// </summary>
    [XmlAttribute]
    public bool ShowAdjustedLengths
    {
        get => (m_Options & LineAnnotationOptions.ShowAdjustedLengths) != 0;
        set => SetOption(LineAnnotationOptions.ShowAdjustedLengths, value);
    }

    /// <summary>
    /// Should the observed length of lines be displayed?
    /// (the units used for the display is determined via another display preference)
    /// </summary>
    [XmlAttribute]
    public bool ShowObservedLengths
    {
        get => (m_Options & LineAnnotationOptions.ShowObservedLengths) != 0;
        set => SetOption(LineAnnotationOptions.ShowObservedLengths, value);
    }

    /// <summary>
    /// Should observed angles be displayed?
    /// </summary>
    [XmlAttribute]
    public bool ShowObservedAngles
    {
        get => (m_Options & LineAnnotationOptions.ShowObservedAngles) != 0;
        set => SetOption(LineAnnotationOptions.ShowObservedAngles, value);
    }

    /// <summary>
    /// Sets option bit(s)
    /// </summary>
    /// <param name="option">The option bit(s) to set</param>
    /// <param name="setting">True to set, false to clear</param>
    void SetOption(LineAnnotationOptions option, bool setting)
    {
        if (setting)
            m_Options |= option;
        else
            m_Options &= (~option);
    }
}