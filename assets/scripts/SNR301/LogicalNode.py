import clr

clr.AddReferenceToFile('iec61850dotnet.dll')

from IEC61850.Client import *
from IEC61850.Common import *

hostname = '192.168.84.200'
connection = IedConnection()

ln = 'SNR301LD0/LLN0'

connection.Connect(hostname)

# S6 LogicalNodeDirectory

print "## Data Objects"
doList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_DATA_OBJECT)
for do in doList:
    print do

print "## Data Sets"
dsList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_DATA_SET)
for ds in dsList:
    print ds
    
print "## BRCB"
brList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_BRCB)
for br in brList:
    print br

print "## URCB"
rpList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_URCB)
for rp in rpList:
	print rp

print "## LCB"
lcbList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_LCB)
for lcb in lcbList:
	print lcb

print "## LOG"
logList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_LOG)
for log in logList:
	print log

print "## SGCB"
sgList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_SGCB)
for sg in sgList:
	print sg

print "## GoCB"
goList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_GoCB)
for go in goList:
	print go

print "## GsCB"
gsList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_GsCB)
for gs in gsList:
	print gs

print "## MSVCB"
msvList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_MSVCB)
for msv in msvList:
	print msv

print "## USVCB"
usvList = connection.GetLogicalNodeDirectory(ln, ACSIClass.ACSI_CLASS_USVCB)
for usv in usvList:
	print usv

# S7 GetAllDataValues	N/A for client

# Data

# S8 GetDataValues

doRef = ln + '.Beh'
doVal = connection.ReadValue(doRef, FunctionalConstraint.ST)

daRef = doRef + '.stVal'
daVal = connection.ReadValue(daRef, FunctionalConstraint.ST)

# S9 SetDataValues

daRef = ln + '.SGCB.EditSG'
from System import UInt32
uint32_mmsValue = MmsValue.__new__.Overloads[UInt32](MmsValue, 1)
connection.WriteValue(daRef, FunctionalConstraint.SP, uint32_mmsValue)

# S10 GetDataDirectory

dataList = connection.GetDataDirectory(doRef)
print "## GetDataDirectory"
for d in dataList:
	print d

# S11 GetDataDefinition the same as GetDataDirectory

connection.Release()