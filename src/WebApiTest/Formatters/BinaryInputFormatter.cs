using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace WebApiTest
{
    public class BinaryInputFormatter : IInputFormatter
    {
        public bool CanRead(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var contentType = context.HttpContext.Request.ContentType;
            if (contentType == null
                || contentType == "quickbird/BinaryArch2v1")
                return true;
            return false;
        }

        public Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var request = context.HttpContext.Request;
            if (request.ContentLength == 0)
            {
                if (context.ModelType.GetTypeInfo().IsValueType)
                    return InputFormatterResult.SuccessAsync(Activator.CreateInstance(context.ModelType));
                else return InputFormatterResult.SuccessAsync(null);
            }

           
            var stream = context.HttpContext.Request.Body;

            return ReadAndDecode(stream);
        }

        public async Task<InputFormatterResult> ReadAndDecode(Stream stream)
        {
            List<Model.Datapoint> list = new List<Model.Datapoint>();
            var LUT = Model.Datapoint.SensorIdLUT; 


            int buffersize = 10000;
            byte[] HexByteBuffer = new byte[buffersize];
            int bufferFilled = 0;
            int bytesRead = 0;

            //Read al lteh data
            do
            {
                bytesRead = await stream.ReadAsync(HexByteBuffer, bufferFilled, buffersize - bufferFilled);
                bufferFilled += bytesRead;
            } while (bytesRead > 0);



            string base64String = System.Text.Encoding.UTF8.GetString(HexByteBuffer).TrimEnd('\0');

            //SocketHandler.MessageAll($"recevied base 64 string /n '{base64String}'"); 

            byte[] buffer =  System.Convert.FromBase64String(base64String); 

            SocketHandler.MessageAll(String.Format("Message of {0} bytes came in", bufferFilled));

            

            int i;

            List<string> sensorsInMessage = new List<string>(); 

            //Read Sensor Header
            for(i = 0; i < bufferFilled; i+= sizeof(short))
            {
                short sensorID = BitConverter.ToInt16(buffer, i);
                SocketHandler.MessageAll($"Sensor id '{sensorID}'\n");

                if (sensorID == 0) //ID of 0 indicates a break in the header
                { break; }

                bool validSendorID = LUT.ContainsKey(sensorID); 
                if (validSendorID == false)
                {
                    SocketHandler.MessageAll($"Invalid sensor ID '{sensorID}', sensor number '{sensorsInMessage.Count}'");
                    return InputFormatterResult.Failure();
                }

                var sensor = Model.Datapoint.SensorIdLUT.FirstOrDefault(lutID => lutID.Key == sensorID);
                sensorsInMessage.Add(sensor.Value); 
            }
            if ((i < bufferFilled) == false)
                return InputFormatterResult.Failure();
            else
            {
                //Skip the delimeter
                i += sizeof(short);
            }

            //Send debug data
            {
                string message = $"Message contains readings for '{sensorsInMessage.Count}' sensors: \n";

                foreach (var sensor in sensorsInMessage)
                {
                    message += $"{sensor}\n";
                }
                SocketHandler.MessageAll(message);
            }


            while (i < buffer.Length)
            {
                Model.Datapoint datapoint = new Model.Datapoint();

                int unixEpoc = BitConverter.ToInt32(buffer, i);
                datapoint.UTCTimestamp = DateTimeOffset.FromUnixTimeSeconds(unixEpoc).UtcDateTime;
                i += 4; //move along by 4 because we read an int; 

                datapoint.chargeState = (Model.ChargeState) buffer[i]; //We are using the same numbers to determine this, so we can just convert directly
                i += 4;

                datapoint.BatteryVoltage = BitConverter.ToSingle(buffer, i);
                i += 4; 

                for(int s = 0; s < sensorsInMessage.Count; s++)
                {
                    string key = sensorsInMessage[s];
                    float value = BitConverter.ToSingle(buffer, i);
                    datapoint.sensorReadings.Add(key, value);
                    i += 4;
                }

                list.Add(datapoint); 
            }

            return InputFormatterResult.Success(list); 
        }


    }
}