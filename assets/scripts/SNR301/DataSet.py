import clr

clr.AddReferenceToFile('iec61850dotnet.dll')

from IEC61850.Client import *
from IEC61850.Common import *

hostname = '192.168.84.200'
connection = IedConnection()

connection.Connect(hostname)

# S12 GetDataSetValues

dsRef = 'SNR301LD0/LLN0.dsAlarm'
ds = connection.GetDataSetValues(dsRef)
print 'size of data set: %d' % ds.GetSize()

# S14 CreateDataSet

from System.Collections.Generic import List
dsElems = List[str]()

dsElems.Add("simpleIOGenericIO/GGIO1.AnIn1.mag.f[MX]")
dsElems.Add("simpleIOGenericIO/GGIO1.AnIn2.mag.f[MX]")

dsRefNew = '@newds'
try:
	connection.CreateDataSet(dsRefNew, dsElems)
except Exception as e:
	print e.message

# S15 DeleteDataSet
try:
	connection.DeleteDataSet(dsRefNew)
except Exception as e:
	print e.message


# S16 GetDataSetDirectory
daList = connection.GetDataSetDirectory(dsRef)
for da in daList:
	print da

connection.Release()
