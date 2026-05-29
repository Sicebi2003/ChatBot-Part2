using chatbot_part_2;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace talkly_part_2
{
    public partial class MainWindow : Window
    {
        private string currentUsername = "";

        // Create objects
        private readonly VoicePlayer audioPlayer = new VoicePlayer();
        private readonly ChatBot chatBotBrain = new ChatBot();

        // Chat history file
        private readonly string logPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChatHistory.txt");

        public MainWindow()
        {
            InitializeComponent();

            audioPlayer.PlayGreeting();

            txtName.Focus();
        }

        // =========================
        // START CHAT
        // =========================

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartChat();
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartChat();
            }
        }

        private void StartChat()
        {
            string username = txtName.Text.Trim();

            // Validate input
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show(
                    "Please enter your name before starting the chat.",
                    "Invalid Name",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            currentUsername = username;

            // File used to store usernames
            string usersFile =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users.txt");

            bool returningUser = false;

            try
            {
                // Check if the file already exists
                if (File.Exists(usersFile))
                {
                    string[] users = File.ReadAllLines(usersFile);

                    // Check if user already exists
                    returningUser = users.Any(user =>
                        user.Equals(username, StringComparison.OrdinalIgnoreCase));
                }

                // Save new user
                if (!returningUser)
                {
                    File.AppendAllText(usersFile,
                        username + Environment.NewLine);
                }
            }
            catch
            {
                // Ignore file errors
            }

            // Switch screens
            StartupScreen.Visibility = Visibility.Collapsed;
            ChatScreen.Visibility = Visibility.Visible;

            // Returning user message
            if (returningUser)
            {
                AppendMessage(
                    "ChatBot",
                    $"Welcome back, {currentUsername}! 🔐\n\n" +
                    $"Great to see you again.\n" +
                    $"Ask me anything about cybersecurity!");
            }
            else
            {
                // First-time user message
                AppendMessage(
                    "ChatBot",
                    $"Welcome {currentUsername}! 🔐\n\n" +
                    $"I'm your Cybersecurity Awareness Assistant.\n" +
                    $"Ask me anything about:\n" +
                    $"• Password Safety\n" +
                    $"• Phishing\n" +
                    $"• Malware\n" +
                    $"• VPNs\n" +
                    $"• Antivirus\n" +
                    $"• Online Privacy");
            }

            txtInput.Focus();
        }

        // send message

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserInput();
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserInput();
            }
        }

        private void ProcessUserInput()
        {
            string userInput = txtInput.Text.Trim();

            // Ignore empty messages
            if (string.IsNullOrWhiteSpace(userInput))
                return;

            // Show user message
            AppendMessage(currentUsername, userInput);

            // Clear input
            txtInput.Clear();

            // Get chatbot response
            string botResponse = chatBotBrain.GetResponse(userInput, currentUsername);

            // Exit command
            if (botResponse == "EXIT_APP")
            {
                Application.Current.Shutdown();
                return;
            }

            // Show chatbot response
            AppendMessage("ChatBot", botResponse);
        }

        // append message

        private void AppendMessage(string sender, string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");

            // Display in UI
            txtChatHistory.Text +=
                $"[{timestamp}] {sender}:\n{message}\n\n";

            // Auto scroll
            ChatScroll.ScrollToEnd();

            // Save to file
            SaveMessageToFile(sender, message);
        }

        // save chat history

        private void SaveMessageToFile(string sender, string message)
        {
            try
            {
                string formattedMessage =
                    $"[{DateTime.Now:dd MMM yyyy HH:mm:ss}] " +
                    $"{sender}: {message}{Environment.NewLine}";

                File.AppendAllText(logPath, formattedMessage);
            }
            catch
            {
                // Ignore file errors
            }
        }
    }
}