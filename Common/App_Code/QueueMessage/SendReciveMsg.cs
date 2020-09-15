using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace Common.App_Code.QueueMessage
{
    public class SendReciveMsg
    {
        public string SendMsgToBotQueue(string message, string queueName)
        {
            string Status = "";
            MessageQueue msMq = null;

            //if (!MessageQueue.Exists(queueName))
            //{
            //    msMq = MessageQueue.Create(queueName);
            //}
            //else
            //{
            //    msMq = new MessageQueue(queueName);
            //}
                       
            try
            {
                msMq = new MessageQueue(queueName);
                msMq.Send(message);
                Status = "1";
            }
            catch (MessageQueueException qEx)
            {
                Status = "0";
            }
            catch (Exception ex)
            {
                Status = "0";
            }
            finally
            {
                msMq.Close();
            }

            return Status;
        }

        public string ReceiveMessageFromQueue(string serverQueue)
        {
            string qMessage = "";
            System.Messaging.Message message;

            MessageQueue messageQueue  = new MessageQueue(serverQueue);

            try
            {
                message = messageQueue.Receive(new TimeSpan(0, 0, 30));
                message.Formatter = new XmlMessageFormatter(
                new String[] { "System.String,mscorlib" });
                qMessage = message.Body.ToString();
            }
            catch (MessageQueueException qEx)
            {
               
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                messageQueue.Close();
            }
            return qMessage;
        }
    }
}
