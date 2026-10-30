#include <elapsedMillis.h>
//test code to send data
const int pie1Pin = A0;
const int pie2Pin = A1;
const int pie3Pin = A2;

int pie1Val = 0;
int pie2Val = 0;
int pie3Val = 0;

//time variables
elapsedMillis sendToUnityTimer;
//could use int but using long for double number of bits
//unsigned means variable value will always be pos. good for millis since time adds up quickly
unsigned long sendToUnityInterval = 40;

void setup() {
  Serial.begin(115200);
  pinMode (pie1Pin, INPUT);
  //looking for full range of values on potentiometer so no pullup
  pinMode (pie2Pin, INPUT);
  pinMode (pie3Pin, INPUT);
}

void loop() {
  readSensors();
  sendSensorDataToUnity();

}

void readSensors(){
  pie1Val = analogRead(pie1Pin);
  pie2Val = analogRead(pie2Pin);
  pie3Val = analogRead(pie3Pin);
}

void sendSensorDataToUnity(){
  //Send Data
  if (sendToUnityTimer > sendToUnityInterval){
    sendToUnityTimer = 0; //Reset Timer
    Serial.print(pie1Val);
    Serial.print(',');
    Serial.print(pie2Val);
    Serial.print(',');
    Serial.println(pie3Val);
  }
 
}
