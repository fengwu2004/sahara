# The library is not visible to end-user

import clr
clr.AddReferenceToFile("iec61850dotnet.dll")
clr.AddReferenceToFile('PcapDotNet.Core.dll')

class ConfigItem:
    def __init__(self, value):
        self.Value = value

conf = {}
conf['AssociateDelayMsecs'] = ConfigItem(0)
conf['ReleaseDelayMsecs'] = ConfigItem(0)

def BeginTest(title, version, date):
    """start capture and print a log line"""
    StartCapture()
    print(title + '\n' + version + '\n' + date + '\n')

def EndTest():
    """stop cature"""
    StopCapture()

def Log(str):
    """TBD"""
    print(str)

def Sleep(ms):
    """TBD"""
    pass

class TestResult:
    PASS = 0
    RUNNING = 1
    FAILED = 2

class IEC61850Error:
    SUCCESS = 0

def CheckAndStopScript():
    pass

from IEC61850.Client import IedConnection

conn = IedConnection()
host = "localhost"

class Result:
    def __init__(self, err):
        self.Error = err

def Associate():
    """Associate service"""
    global conn, host
    conn.Connect(host, 102)
    return Result(IEC61850Error.SUCCESS)
    
def Release():
    """Release service"""
    global conn
    conn.Release()
    return Result(IEC61850Error.SUCCESS)

def Abort():
    """Abort service"""
    global conn
    conn.Abort()
    return Result(IEC61850Error.SUCCESS)

# Capture

from PcapDotNet.Core import *

allDevices = LivePacketDevice.AllLocalMachine
selectedDevice = allDevices[1]
communicator = selectedDevice.Open(65536, # portion of the packet to capture
                                          # 65536 guarantees that the whole packet will be captured on all the link layers
                                   PacketDeviceOpenAttributes.Promiscuous, # promiscuous mode
                                   1000) # read timeout

def StartCapture():
    global communicator
    dumpFile = communicator.OpenDump("dumpfile.pcap")
    import thread
    thread.start_new_thread(communicator.ReceivePackets, (0, dumpFile.Dump))

def StopCapture():
    global communicator
    communicator.Break()
