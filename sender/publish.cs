using System;
using System.Collections.Generic;
using System.Text;
using SolaceSystems.Solclient.Messaging;

namespace sender
{
   public class Publish
    {
        public string Host { get; set; }
        public string VPNName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        const int DefaultReconnectRetries = 3;
        //public String Method;
        //public String CommunicationText;
        //public String Topic;


        public Publish(String Host, String Vpn, String UserName, String Password)
        {
            Host = Host;
            VPNName = Vpn;
            UserName = UserName;
            Password = Password;
           
        }

       public void Run()
        {
            // Validate parameters
            ContextFactoryProperties cfp = new ContextFactoryProperties()
            {
                SolClientLogLevel = SolLogLevel.Warning
            };
            cfp.LogToConsoleError();
            ContextFactory.Instance.Init(cfp);
            try
            {
                using (IContext context = ContextFactory.Instance.CreateContext(new ContextProperties(), null))
                {
                  

                    // Run the application within the context and against the host
                   

                
                if (context == null)
                {
                    throw new ArgumentException("Solace Systems API context Router must be not null.", "context");
                }
                if (string.IsNullOrWhiteSpace(Host))
                {
                    throw new ArgumentException("Solace Messaging Router host name must be non-empty.", "host");
                }
                if (string.IsNullOrWhiteSpace(VPNName))
                {
                    throw new InvalidOperationException("VPN name must be non-empty.");
                }
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    throw new InvalidOperationException("Client username must be non-empty.");
                }

                // Create session properties
                SessionProperties sessionProps = new SessionProperties()
                {
                    Host = Host,
                    VPNName = VPNName,
                    UserName = UserName,
                    Password = Password,
                    ReconnectRetries = DefaultReconnectRetries
                };

                // Connect to the Solace messaging router
                Console.WriteLine("Connecting as {0}@{1} on {2}...", UserName, VPNName, Host);
                using (ISession session = context.CreateSession(sessionProps, null, null))
                {
                    ReturnCode returnCode = session.Connect();
                    if (returnCode == ReturnCode.SOLCLIENT_OK)
                    {
                        Console.WriteLine("Session successfully connected.");
                        string Topic = "toplic";
                        string Message = "message";
                        PublishMessage(session, Topic, Message);
                    }
                    else
                    {
                        Console.WriteLine("Error connecting, return code: {0}", returnCode);
                    }
                }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown: {0}", ex.Message);
            }
            finally
            {
                // Dispose Solace Systems Messaging API
                ContextFactory.Instance.Cleanup();
            }
        }
        private void PublishMessage(ISession session,string Topic,string Message)
        {
            // Create the message
            using (IMessage message = ContextFactory.Instance.CreateMessage())
            {
                message.Destination = ContextFactory.Instance.CreateTopic("tutorial/topic");
                // Create the message content as a binary attachment
                message.BinaryAttachment = Encoding.ASCII.GetBytes("Sample Message");

                // Publish the message to the topic on the Solace messaging router
                Console.WriteLine("Publishing message...");
                ReturnCode returnCode = session.Send(message);
                if (returnCode == ReturnCode.SOLCLIENT_OK)
                {
                    Console.WriteLine("Done.");
                }
                else
                {
                    Console.WriteLine("Publishing failed, return code: {0}", returnCode);
                }
            }
        }
       
    }
}
