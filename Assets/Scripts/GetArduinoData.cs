using UnityEngine;
using System.IO.Ports;
using System;  
//using signals for external libraries

//class starts and ends code here
public class GetArduinoData : MonoBehaviour
{
    public string portName = "/dev/cu.usbmodem101"; // Replace with your Arduino's port name
    public int baudRate = 115200; // Replace with your Arduino's baud rate
    //SerialPort stream sets up instance of data stream from arduino
    private SerialPort stream;
    //global variables
    public int pie1Val;
    public int pie2Val;
    public int pie3Val;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stream = new SerialPort(portName, baudRate);
        stream.ReadTimeout = 50; // Set a read timeout to avoid blocking the main thread
        try
        {
            stream.Open();
            Debug.Log("Serial port opened successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to open serial port: " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (stream != null && stream.IsOpen)
        {
            // Read data from the serial port
            try
            {
                string data = stream.ReadLine();
                string[] values = data.Split(',');
                //split data at ,

                if (values.Length >= 3)
                //make sure three values
                {
                    pie1Val = int.Parse(values[0]);
                    pie2Val = int.Parse(values[1]);
                    pie3Val = int.Parse(values[2]);
                    //turn data back into integer
                }
            }
            catch (TimeoutException)
            {
                // Handle timeout exception if no data is received within the specified time
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading from serial port: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        // Close the serial port when the application quits
        if (stream != null && stream.IsOpen)
        {
            stream.Close();
            Debug.Log("Serial port closed.");
        }
    }
}