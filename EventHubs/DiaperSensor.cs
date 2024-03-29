﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learning.EventHubs.Model;

public abstract class BaseModel
{
    /// <summary>
    /// Event trigger in Azure Stream Analytics
    /// </summary>
    public string Event
    {
        get { return GetType().Name; }
    }
}

public class DiaperSensor : BaseModel
{
    /// <summary>
    /// Id for the sensor.
    /// </summary>
    public string SensorId { get; set; }

    /// <summary>
    /// The version of the firmware running on the sensor.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Status value containing battery percentage and flags.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Remaining battery.
    /// </summary>
    public double Battery { get; set; }

    /// <summary>
    /// Flag for whether Hunmidity and Temperature sensors are enabled.
    /// </summary>
    public bool FlagHumidTempEnabled { get; set; }

    /// <summary>
    /// Flag for whether Accelerometer is enabled.
    /// </summary>
    public bool FlagAccelerometerEnabled { get; set; }

    /// <summary>
    /// Flag for whether a fall have occured.
    /// </summary>
    public bool FlagFallIndicator { get; set; }

    /// <summary>
    /// Flag for whether there have been any movement since last sensor log.
    /// </summary>
    public bool FlagMicroMovementIndicator { get; set; }

    /// <summary>
    /// The humidity values received from the sensor, measured in percentage.
    /// </summary>
    public double Humid1 { get; set; }
    public double Humid2 { get; set; }
    public double Humid3 { get; set; }
    public double Humid4 { get; set; }

    /// <summary>
    /// The temperature values received from the sensor, measured in celsius.
    /// </summary>
    public double CTemp1 { get; set; }
    public double CTemp2 { get; set; }
    public double CTemp3 { get; set; }
    public double CTemp4 { get; set; }

    /// <summary>
    /// The axis values received from the sensor.
    /// </summary>
    public double AxisX { get; set; }
    public double AxisY { get; set; }
    public double AxisZ { get; set; }

    /// <summary>
    /// Raw payload used for debugging and duplicate importing.
    /// </summary>
    public string Payload { get; set; }

    /// <summary>
    /// Timestamp for when the Input function received the log
    /// </summary>
    public DateTime UtcInputTimestamp { get; set; }

    /// <summary>
    /// Id for the gateway who received the sensor log.
    /// </summary>
    public string GatewayId { get; set; }

    /// <summary>
    /// Gateway provider who received the sensor log.
    /// </summary>
    public string GatewayProvider { get; set; }

    /// <summary>
    /// Id generated by the gateway for the specific package (collection of events).
    /// </summary>
    public string GatewayPackageId { get; set; }

    /// <summary>
    /// Received signal strength indicator, between sensor and gateway.
    /// </summary>
    public int GatewayRssi { get; set; }

    /// <summary>
    /// The timestamp (in UTC) from the gateway.
    /// </summary>
    public DateTime UtcGatewayTimestamp { get; set; }

}
