# Solar Server 

1. Accepts  uploads from the sensors
2. Provides REST api to query the readings


## Uploads 
The URL is http://{serverURI}/api/batchupload/{nodeID}, for example http://iotupload.quickbird.co.uk/api/batchupload/3
It uses special format - 
'{"Content-Type":"quickbird/BinaryArch2v1"}'
 The content is a memory representation of the following memory dump in BASE64 form: 

 Header denotes sensor types being submitted from this sensor, all represented as shorts. We have the following sensor types: 
~~~C
enum types {      
	Light = 4, 
        Pressure_internal_open = 5, 
        Humidity_internal_open = 6,
        Temp_internal_open = 7, 
        Temp_external_air = 8, 
        Humid_external_air = 9, 
        Pressure_external_air = 10, 
        Temp_soil_1 = 11,
        Temp_soil_2 = 12,
        Temp_soil_3 = 13,
        Soil_moisture_1 = 14, 
        Soil_moisture_2 = 15, 
        Soil_moisture_3 = 16, 
        Surface_moisture = 17, 
        Surface_temp = 18,
}
~~~
A single frame may some or all of the sensors. All datapoints must have the same structure: 
~~~C
    unsigned int unixTime; 
    char chargeState;
    float batteryVoltage; 
~~~
Sensors listed in the header. 

All the data is memory-copy from a C array. 

## Downloads 
The endpoint http://{ServerURI}/api/readings_to_graph/{nodeId} is used to serve the node dashboard
The server also has an in-built graph, which queries the controller at http://iotupload.quickbird.co.uk/api/Nodes
It is a bit silly and redundant. 
