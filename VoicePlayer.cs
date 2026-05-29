using System;
using System.IO;
using System.Media;

namespace chatbot_part_2
{
    // Handles asynchronous audio playback for the application's media files.

    public class VoicePlayer 
    {
        // Safely attempts to locate and play the greeting .wav file.
        // Uses a try-catch block to prevent UI crashes if the file is missing.
        public void PlayGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Vgreeting.wav");
                if (File.Exists(path))
                {
                    SoundPlayer voice_player = new SoundPlayer(path);
                    voice_player.LoadAsync();
                    voice_player.Play();
                }
            }
            catch
            {
            }
        }//
    }
}