import clr

clr.AddReferenceToFile('iec61850dotnet.dll')

from IEC61850.Client import *
from IEC61850.Common import *

hostname = '192.168.84.200'
connection = IedConnection()

ld = 'SNR301LD0'

## S1-ServerDirectory

connection.Connect(hostname)

lnList = connection.GetLogicalDeviceDirectory(ld)
for ln in lnList:
    print ln

connection.Release()