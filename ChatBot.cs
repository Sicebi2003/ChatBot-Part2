using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace talkly_part_2
{
    public class ChatBot
    {
        Random random = new Random();

        public string currentTopic = "";
        public string memoryFile = "";

        // =========================
        // USER MEMORY
        // =========================

        public void SetUserMemory(string userName)
        {
            memoryFile = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"{userName}_memory.txt");
        }

        // =========================
        // CYBERSECURITY RESPONSES
        // =========================

        Dictionary<string, string[]> cyberResponses =
            new Dictionary<string, string[]>()
        {
            {
                "phishing",
                new string[]
                {
                    "Phishing uses fake emails or websites to steal personal information.",
                    "Cybercriminals pretend to be trusted companies during phishing attacks.",
                    "Never click suspicious email links because they may steal passwords.",
                    "Phishing attacks often create urgency to trick victims."
                }
            },

            {
                "malware",
                new string[]
                {
                    "Malware is harmful software created to damage or exploit devices.",
                    "Viruses can spread between files and slow down your computer.",
                    "Trojans disguise themselves as safe software to trick users.",
                    "Spyware secretly monitors your activity and steals information.",
                    "Ransomware encrypts files and demands payment to unlock them.",
                    "Adware displays unwanted advertisements and may track browsing habits.",
                    "Worms can spread across networks without user interaction.",
                    "Rootkits hide deep inside systems and are difficult to detect.",
                    "Keeping antivirus software updated helps protect against malware.",
                    "Avoid downloading files from unknown websites to reduce malware risks."
                }
            },

            {
                "passwords",
                new string[]
                {
                    "Strong passwords should contain letters, numbers, and symbols.",
                    "Never reuse passwords across multiple websites.",
                    "Password managers help keep passwords secure.",
                    "Enable two-factor authentication for extra protection."
                }
            }
        };

        // =========================
        // CYBERSECURITY TIPS
        // =========================

        Dictionary<string, string[]> cyberTips =
            new Dictionary<string, string[]>()
        {
            {
                "phishing",
                new string[]
                {
                    "Tip: Always check the sender's email address before clicking links.",
                    "Tip: Never enter passwords through email links — go directly to the website.",
                    "Tip: Look for urgent messages like 'act now' — these are common scams.",
                    "Tip: Hover over links to see where they really lead."
                }
            },

            {
                "malware",
                new string[]
                {
                    "Tip: Keep your antivirus software updated at all times.",
                    "Tip: Avoid downloading files from unknown websites.",
                    "Tip: Scan USB drives before opening files.",
                    "Tip: Regular updates help patch security vulnerabilities."
                }
            },

            {
                "passwords",
                new string[]
                {
                    "Tip: Use at least 12 characters with symbols and numbers.",
                    "Tip: Never reuse the same password on multiple accounts.",
                    "Tip: Use a password manager to store passwords safely.",
                    "Tip: Enable two-factor authentication whenever possible."
                }
            }
        };

        // =========================
        // KEYWORDS
        // =========================

        Dictionary<string, string[]> topicKeywords =
            new Dictionary<string, string[]>()
        {
            {
                "phishing",
                new string[]
                {
                    "phishing",
                    "fake email",
                    "suspicious email",
                    "scam"
                }
            },

            {
                "malware",
                new string[]
                {
                    "virus",
                    "trojan",
                    "spyware",
                    "adware",
                    "malware",
                    "ransomware",
                    "worm",
                    "rootkit",
                    "infected",
                    "antivirus",
                    "hack"
                }
            },

            {
                "passwords",
                new string[]
                {
                    "password",
                    "passwords",
                    "login",
                    "authentication",
                    "2fa"
                }
            }
        };

        // =========================
        // MAIN CHATBOT RESPONSE
        // =========================

        public string GetResponse(string message, string userName)
        {
            message = message.ToLower();

            // Exit app
            if (message == "exit")
            {
                return "EXIT_APP";
            }

            // Favorite topic
            if (message.Contains("favorite topic") ||
                message.Contains("favourite topic") ||
                message.Contains("most asked") ||
                message.Contains("history"))
            {
                string favorite = ReadFavoriteTopic();

                if (!string.IsNullOrEmpty(favorite))
                {
                    return $"Based on your history, your favorite topic is '{favorite}' because you ask about it the most!";
                }

                return "I do not have enough data to know your favorite topic yet.";
            }

            // Tips request
            if (message.Contains("tip") || message.Contains("tips"))
            {
                if (!string.IsNullOrEmpty(currentTopic) &&
                    cyberTips.ContainsKey(currentTopic))
                {
                    string[] tips = cyberTips[currentTopic];

                    return tips[random.Next(tips.Length)];
                }

                return "Ask me about phishing, malware, or passwords first.";
            }

            // Detect emotion + topic
            string sentiment = DetectSentiment(message);
            bool moreInfo = IsFollowUp(message);
            string topic = DetectTopic(message);

            // Follow-up questions
            if (string.IsNullOrEmpty(topic) &&
                moreInfo &&
                !string.IsNullOrEmpty(currentTopic))
            {
                topic = currentTopic;
            }

            // Build response
            if (!string.IsNullOrEmpty(topic))
            {
                currentTopic = topic;

                SaveTopic(topic);

                return BuildResponse(topic, sentiment, userName);
            }

            // Emotion support
            if (!string.IsNullOrEmpty(sentiment))
            {
                return GetSentimentSupport(sentiment, userName) +
                       " What would you like to know about?";
            }

            // Default response
            return "I'm not sure I understand.\n\nTry asking about:\n• Passwords\n• Malware\n• Phishing";
        }

        // =========================
        // DETECT TOPIC
        // =========================

        private string DetectTopic(string message)
        {
            foreach (var topic in topicKeywords)
            {
                if (topic.Value.Any(word => message.Contains(word)))
                {
                    return topic.Key;
                }
            }

            return "";
        }

        // =========================
        // BUILD RESPONSE
        // =========================

        private string BuildResponse(
            string topic,
            string sentiment,
            string userName)
        {
            string[] responses = cyberResponses[topic];

            string response =
                responses[random.Next(responses.Length)];

            string support =
                GetSentimentSupport(sentiment, userName);

            if (!string.IsNullOrEmpty(support))
            {
                return support + "\n\n" + response;
            }

            return response;
        }

        // =========================
        // DETECT SENTIMENT
        // =========================

        private string DetectSentiment(string message)
        {
            if (message.Contains("worried") ||
                message.Contains("afraid") ||
                message.Contains("nervous") ||
                message.Contains("scared"))
            {
                return "worried";
            }

            if (message.Contains("angry") ||
                message.Contains("frustrated") ||
                message.Contains("confused") ||
                message.Contains("lost"))
            {
                return "frustrated";
            }

            return "";
        }

        // =========================
        // SUPPORTIVE RESPONSES
        // =========================

        private string GetSentimentSupport(
            string sentiment,
            string userName)
        {
            if (sentiment == "worried")
            {
                return $"Hey {userName}, cybersecurity can feel overwhelming, but don't worry, I am here to help keep you safe.";
            }

            if (sentiment == "frustrated")
            {
                return $"Hey {userName}, I know tech can be frustrating. Let's solve this together step by step.";
            }

            return "";
        }

        // =========================
        // FOLLOW-UP DETECTION
        // =========================

        private bool IsFollowUp(string message)
        {
            return message.Contains("explain more") ||
                   message.Contains("more details") ||
                   message.Contains("tell me more");
        }

        // =========================
        // SAVE TOPIC MEMORY
        // =========================

        private void SaveTopic(string topic)
        {
            if (string.IsNullOrEmpty(memoryFile))
                return;

            try
            {
                File.AppendAllText(
                    memoryFile,
                    topic + Environment.NewLine);
            }
            catch
            {
            }
        }

        // =========================
        // READ FAVORITE TOPIC
        // =========================

        private string ReadFavoriteTopic()
        {
            if (string.IsNullOrEmpty(memoryFile) ||
                !File.Exists(memoryFile))
            {
                return "";
            }

            try
            {
                string[] topics =
                    File.ReadAllLines(memoryFile);

                if (topics.Length == 0)
                    return "";

                return topics
                    .GroupBy(t => t)
                    .OrderByDescending(g => g.Count())
                    .First()
                    .Key;
            }
            catch
            {
                return "";
            }
        }
    }
}