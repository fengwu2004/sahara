import clr

clr.AddReferenceToFile('iec61850dotnet.dll')

from IEC61850.Client import *
from IEC61850.Common import *

hostname = '127.0.0.1'
connection = IedConnection()

## S2-Associate

connection.Connect(hostname)
connection.Release()

## S3-Abort

# program hang on abort
connection.Connect(hostname)
connection.Abort()

## S4-Release

connection.Connect(hostname)
connection.Release()