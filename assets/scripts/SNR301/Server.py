import clr

clr.AddReferenceToFile('iec61850dotnet.dll')

from IEC61850.Client import *
from IEC61850.Common import *

hostname = '192.168.84.200'
connection = IedConnection()

ln = 'SNR301LD0/LLN0'

connection.Connect(hostname)

doRef = ln + '.Beh'
dd = connection.GetDataDirectory(doRef)

for da in dd:
	print da

connection.Release()