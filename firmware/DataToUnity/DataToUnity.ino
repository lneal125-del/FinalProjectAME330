#include <elapsedMillis.h>
//test code to send data
const int pie1Pin = A0;
const int pie2Pin = A1;
const int pie3Pin = A2;
const int pie4Pin = A3;
const int pie5Pin = A4;
const int pie6Pin = A5;
const int pie7Pin = A6;
const int pie8Pin = A7;

int pie1Val = 0;
int pie2Val = 0;
int pie3Val = 0;
int pie4Val = 0;
int pie5Val = 0;
int pie6Val = 0;
int pie7Val = 0;
int pie8Val = 0;

//time variables
elapsedMillis sendToUnityTimer;
//could use int but using long for double number of bits
//unsigned means variable value will always be pos. good for millis since time adds up quickly
unsigned long sendToUnityInterval = 40;

void setup() {
  Serial.begin(115200);
  pinMode (pie1Pin, INPUT);
  //looking for full range of values so no pullup
  pinMode (pie2Pin, INPUT);
  pinMode (pie3Pin, INPUT);
  pinMode (pie4Pin, INPUT);
  pinMode (pie5Pin, INPUT);
  pinMode (pie6Pin, INPUT);
  pinMode (pie7Pin, INPUT);
  pinMode (pie8Pin, INPUT);
}

void loop() {
  readSensors();
  sendSensorDataToUnity();

}

void readSensors(){
  pie1Val = analogRead(pie1Pin);
  pie2Val = analogRead(pie2Pin);
  pie3Val = analogRead(pie3Pin);
  pie4Val = analogRead(pie4Pin);
  pie5Val = analogRead(pie5Pin);
  pie6Val = analogRead(pie6Pin);
  pie7Val = analogRead(pie7Pin);
  pie8Val = analogRead(pie8Pin);
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
    Serial.print(',');
    Serial.println(pie4Val);
    Serial.print(',');
    Serial.println(pie5Val);
    Serial.print(',');
    Serial.println(pie6Val);
    Serial.print(',');
    Serial.println(pie7Val);
    Serial.print(',');
    Serial.println(pie8Val);
  }
 
}
