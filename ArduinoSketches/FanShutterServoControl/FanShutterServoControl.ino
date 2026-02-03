/* Sweep
 by BARRAGAN <http://barraganstudio.com>
 This example code is in the public domain.

 modified 8 Nov 2013
 by Scott Fitzgerald
 https://www.arduino.cc/en/Tutorial/LibraryExamples/Sweep
*/

#include <Servo.h>

Servo fanServoLeft;  
Servo fanServoRight;  

#define SERVO_L_PIN 3
#define SERVO_R_PIN 5

#define SERVO_L_POS_CLOSE 140
#define SERVO_L_POS_OPEN 80

#define SERVO_R_POS_CLOSE 40
#define SERVO_R_POS_OPEN 100

bool isOpen = false;

void setup() {
  Serial.begin(115200);

  fanServoLeft.attach(SERVO_L_PIN);  
  fanServoRight.attach(SERVO_R_PIN);  

  setOpen(false);
}

void setOpen(bool setOpen)
{
  isOpen = setOpen;
  if (isOpen)
  {
    fanServoLeft.write(SERVO_L_POS_OPEN); 
    fanServoRight.write(SERVO_R_POS_OPEN); 
  }
  else
  {
    fanServoLeft.write(SERVO_L_POS_CLOSE); 
    fanServoRight.write(SERVO_R_POS_CLOSE);
  }
}

char inputBuffer;

void serialControl()
{
  if (Serial.available()) {
    Serial.readBytes(&inputBuffer, 1);
    Serial.write(inputBuffer);
    setOpen(inputBuffer);
  }
}

void debugControl()
{
  if (Serial.available()) {
    while(Serial.available() > 0)
    {
      Serial.read();
    }
    setOpen(!isOpen);
    Serial.println(isOpen ? "Open" : "Close");
  }
}

void loop() {
  serialControl();
  delay(10);
  // debugControl();
}
