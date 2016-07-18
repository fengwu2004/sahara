import clr

clr.AddReferenceToFile('iec61850dotnet.dll')

from IEC61850.Client import *
from IEC61850.Common import *

hostname = '192.168.84.200'
connection = IedConnection()

ln = 'SNR301LD0/LLN0'

connection.Connect(hostname)

# Setting group control
sgcbRef = 'SNR301PROT/LLN0.SGCB'

# S18	SelectActiveSG	TP	Y	Y	since version 0.8.2
from System import UInt32
uint32_mmsValue = MmsValue.__new__.Overloads[UInt32](MmsValue, 1)
try:
	connection.WriteValue(sgcbRef + '.ActSG', FunctionalConstraint.SP, uint32_mmsValue)
except Exception as e:
	print e.message

# S19	SelectEditSG	TP	Y	Y	
try:
	connection.WriteValue(sgcbRef + '.EditSG', FunctionalConstraint.SP, uint32_mmsValue)
except Exception as e:
	print e.message

# S20	SetSGValues		TP	Y	Y	
sgRef = 'SNR301PROT/LLN0.Lenth.setMag.f'
float_mmsValue = MmsValue.__new__.Overloads[float](MmsValue, 1.0)

try:
	connection.WriteValue(sgRef, FunctionalConstraint.SE, float_mmsValue)
except Exception as e:
	print e.message
	
# S21	ConfirmEditSGValues	TP	Y	Y	
try:
	connection.WriteValue(sgcbRef + '.CnfEdit', FunctionalConstraint.SP, MmsValue(True))
except Exception as e:
	print e.message

# S22	GetSGValues	TP	Y	Y	
connection.ReadValue(sgRef, FunctionalConstraint.SG)

# S23	GetSGCBValues	TP	Y	Y
connection.ReadValue(sgcbRef, FunctionalConstraint.SP)
