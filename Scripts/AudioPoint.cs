using Godot;

/**
 * Simple class to play a audio stream at a world point.
 * Assumes it has its own audio stream player and audio stream setup.
 */
public class AudioPoint : AudioStreamPlayer3D
{
    public void OnEvent()
    {
        Play();
    }
}