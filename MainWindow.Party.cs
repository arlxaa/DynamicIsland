using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DynamicIsland;

/// <summary>
/// Party mode: a "jedag-jedug" rainbow. Instead of tinting the equalizer and the progress bar with the album
/// art's colour, the shared accent brush cycles smoothly through the full spectrum, over and over, so the
/// visualizer bars and the progress fill throb in changing colours while music plays. Toggled from the
/// right-click menu; it costs nothing while it's off, and only one looping animation while it's on.
/// </summary>
public partial class MainWindow
{
    private bool _partyMode;
    private AnimationClock? _partyClock;

    /// <summary>One lap of the rainbow. Short enough to feel like a beat, long enough not to strobe.</summary>
    private static readonly Duration PartyLap = new(TimeSpan.FromSeconds(6));

    private void ToggleParty()
    {
        _partyMode = !_partyMode;
        if (_partyMode)
            StartParty();
        else
            StopParty();
    }

    /// <summary>Runs the looping rainbow on the shared accent brush.</summary>
    private void StartParty()
    {
        // Six evenly-spaced hues plus a seventh that lands back on the first, so the loop is seamless.
        var rainbow = new ColorAnimationUsingKeyFrames
        {
            Duration = PartyLap,
            RepeatBehavior = RepeatBehavior.Forever,
        };
        Color[] hues =
        {
            Color.FromRgb(0xFF, 0x3B, 0x30), // red
            Color.FromRgb(0xFF, 0x9F, 0x0A), // orange
            Color.FromRgb(0xFF, 0xD6, 0x0A), // yellow
            Color.FromRgb(0x34, 0xC7, 0x59), // green
            Color.FromRgb(0x0A, 0x84, 0xFF), // blue
            Color.FromRgb(0xBF, 0x5A, 0xF2), // purple
            Color.FromRgb(0xFF, 0x3B, 0x30), // back to red
        };
        for (int i = 0; i < hues.Length; i++)
        {
            double at = (double)i / (hues.Length - 1);
            rainbow.KeyFrames.Add(new LinearColorKeyFrame(
                hues[i], KeyTime.FromTimeSpan(TimeSpan.FromSeconds(PartyLap.TimeSpan.TotalSeconds * at))));
        }

        _partyClock = rainbow.CreateClock();
        _accentBrush.ApplyAnimationClock(SolidColorBrush.ColorProperty, _partyClock);
    }

    /// <summary>Stops the rainbow and eases the accent back to the current album art colour.</summary>
    private void StopParty()
    {
        if (_partyClock is not null)
        {
            _accentBrush.ApplyAnimationClock(SolidColorBrush.ColorProperty, null);
            _partyClock = null;
        }
        _accentBrush.BeginAnimation(SolidColorBrush.ColorProperty,
            new ColorAnimation(_accentTarget, TimeSpan.FromMilliseconds(450)));
    }
}
