from context import *

#######################################
# Manual script configuration section #
#######################################

associateDelayMsecs = conf['AssociateDelayMsecs'].Value
releaseDelayMsecs   = conf['ReleaseDelayMsecs'].Value

#############################################
# No manual changes needed beyond this line #
#############################################

title   = "Ass1"
version = "1.3.t2"
date    = "10 Apr 2014"

# Version History:
# ----------------
# 1.3       xx.xx.2012  GridClone   Initial version.
# 1.3.t1    20.11.2012  Peter       Bugfix.
# 1.3.t2    10.04.2014  Peter       Bugfix at usage of associateDelayMsecs and releaseDelayMsecs.

testStatus = TestResult.RUNNING

if ( associateDelayMsecs > 0 ):
    Log( "Setup Time for Associate(): " + str(associateDelayMsecs) )
if ( releaseDelayMsecs > 0 ):
    Log( "Setup Time for Release(): " + str(releaseDelayMsecs) )

########################
# start of actual test #
########################
BeginTest(title, version, date)

# A1. Configure the SIMULATOR and DUT with the correct association and authentication parameters
# Nothing to do in script.
Log("A1. Configure the SIMULATOR and DUT with the correct association and authentication parameters")

# A4. (Do once then) Repeat step 2 and 3 250 times
for i in range(1, 252):
    CheckAndStopScript()

    # A2. Client request Associate
    Log(" A2. Associate attempt no. " + str(i))
    connect = Associate()

    # R2. DUT sends Associate Response+
    if ( connect.Error != IEC61850Error.SUCCESS ):
        testStatus = TestResult.FAIL
        Log( "ERROR: Associate() in cycle " + str(i) + " failed." )
        # wait a moment and continue with next cycle
        Sleep( 2000 )
        continue
    else:
        Log("R2. DUT sends Associate Response+")

    if ( associateDelayMsecs > 0 ):
        Sleep( associateDelayMsecs )
        
    # A3. Client request Release
    Log("A3. Client request Release")
    Log("Release attempt no." + str(i))
    disconnect = Release()

    # R3. DUT sends Release Response+
    if ( disconnect.Error != IEC61850Error.SUCCESS ):
        testStatus = TestResult.FAIL
        Log( "ERROR: Release() in cycle " + str(i) + " failed." )
    else:
        Log(" R3. DUT sends Release Response+")

    if ( releaseDelayMsecs > 0 ):
        Sleep( releaseDelayMsecs )
        

########################
#  end of actual test  #
########################
if ( testStatus == TestResult.RUNNING ):
    FinalResult = TestResult.PASS
else:
    FinalResult = testStatus

EndTest()

##############################################
# This Test Procedure is conducted for Conformance Test
# Conformance Block         : Basic Exchange
# Name                      : Associate and release a TPAA association (Ass1)
# Description               : Associate and release a TPAA association (IEC 61850-7-2 clause 7.4)
# Priority                  : Mandatory
# Version                   : 1
# Author                    : Haris Hamdani
# Test Procedures Reference : Conformance Test Procedures for Server Devices with IEC 61850-8-1 interface Revision 2.3
#
# References                :
#   1. IEC 61850-7-2 clause 7.4
#   2. IEC 61850-8-1 clause 10.2
#
# Test description          :
#   1. Configure the SIMULATOR and DUT with the correct association and authentication parameters
#   2. Client request Associate
#   3. Client request Release
#   4. Repeat step 2 and 3 250 times
#
# Expected Result           :
#   2. DUT sends Associate Response+
#   3. DUT sends Release Response+
#
##############################################