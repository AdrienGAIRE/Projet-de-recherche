from __future__ import absolute_import
from __future__ import print_function

import optparse
import socket
from threading import Thread
from sumolib import checkBinary
import traci
import traci.constants as tc


#TCP informations
TCP_IP = '127.0.0.1'
TCP_PORT = 5005
BUFFER_SIZE = 1024

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((TCP_IP, TCP_PORT))


#Threads to check for informations from the Unity Simulation
class SocketThread(Thread):

    def __init__(self, conn):
        Thread.__init__(self)
        self.conn = conn
        self._isRunning = True

    def run(self):
        while self._isRunning:
            data = conn.recv(BUFFER_SIZE).decode()
            if data != "":
                print("received data:", data)
                if data == "Fin\r":
                    conn.close()
                    self.stop()

    def stop(self):
        self._isRunning = False

    def send(self, message):
        conn.send(message.encode("utf-8"))


# Listening for connection from the Unity Socket
s.listen(1)
conn, addr = s.accept()
print('Connection address:', addr)
threadUnity = SocketThread(conn)
threadUnity.start()


def run():
    """execute the TraCI control loop"""
    step = 0
    vehicles = []
    # persons = []
    while traci.simulation.getMinExpectedNumber() > 0:
	

        # Adding the new vehicle to the list of vehicles
        if len(traci.simulation.getLoadedIDList()) > 0:
            vehicles = vehicles + traci.simulation.getLoadedIDList()

        # Removing from the list of vehicles those who were removed
        if len(traci.simulation.getArrivedIDList()) > 0:
            vehicles = [x for x in vehicles if x not in traci.simulation.getArrivedIDList()]
"""
Another solution would be to load the vehicles in a list and the persons in another. That way, the persons are taken in account. This is to work on.
        # Adding the vehicles to the list
        if len(traci.vehicle.getIDList()) > 0:
            vehicles = traci.vehicle.getIDList()

        # Adding the persons to the list
        if len(traci.person.getIDList()) > 0:
            persons = traci.person.getIDList()
"""
        
        print("Time:")
        print(traci.simulation.getCurrentTime())
        for veh in vehicles:
            print(traci.vehicle.getPosition(veh))
            # The message is in the format "id:x;y|". The | marks the separation between two vehicles.
            message = veh + ":" + str(round(traci.vehicle.getPosition(veh)[0], 2)) + ";" + str(round(traci.vehicle.getPosition(veh)[1], 2)) + "|"
            threadUnity.send(message)
"""
        for per in persons:
            print(traci.person.getPosition(per))
            # The message is in the format "id:x;y|". The | marks the separation between two vehicles.
            message = per + ":" + str(round(traci.vehicle.getPosition(veh)[0], 2)) + ";" + str(round(traci.vehicle.getPosition(veh)[1], 2)) + "|"
            threadUnity.send(message)
"""
        traci.simulationStep()
        step += 1
    traci.close()
    sys.stdout.flush()


def get_options():
    optParser = optparse.OptionParser()
    optParser.add_option("--nogui", action="store_true",
                         default=False, help="run the commandline version of sumo")
    options, args = optParser.parse_args()
    return options


# this is the main entry point of this script
if __name__ == "__main__":
    options = get_options()

    # this script has been called from the command line. It will start sumo as a
    # server, then connect and run
    if options.nogui:
        sumoBinary = checkBinary('sumo')
    else:
        sumoBinary = checkBinary('sumo-gui')

    # this is the normal way of using traci. sumo is started as a
    # subprocess and then the python script connects and runs
    traci.start([sumoBinary, "-c", "Project_orio.sumocfg"])

    run()