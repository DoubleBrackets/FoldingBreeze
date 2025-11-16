/*
v. 0.0.1

Intended to be used with Arduino Nano Every

Communicates with Unity over Serial, transmitting rotation, acceleration, 
*/

// Basic demo for readings from Adafruit BNO08x
#include <Adafruit_BNO08x.h>

// For SPI mode, we need a CS pin
#define BNO08X_CS 10
#define BNO08X_INT 9

// For SPI mode, we also need a RESET
//#define BNO08X_RESET 5
// but not for I2C or UART
#define BNO08X_RESET -1

Adafruit_BNO08x bno08x(BNO08X_RESET);
sh2_SensorValue_t sensorValue;

// Switches
#define FAN_CLOSE_PIN 3
#define FAN_OPEN_PIN 4

int fanOpenBtn = 0;
int fanCloseBtn = 0;

void setup_gyro() {

  // Serial.println("Adafruit BNO08x test!");

  // Try to initialize!
  while (!bno08x.begin_I2C()) {
    //if (!bno08x.begin_UART(&Serial1)) {  // Requires a device with > 300 byte UART buffer!
    //if (!bno08x.begin_SPI(BNO08X_CS, BNO08X_INT)) {
    Serial.println("Failed to find BNO08x chip, trying again in 2s");
    delay(2000);
    Serial.println("Trying again");
  }
  Serial.println("BNO08x Found!");

  for (int n = 0; n < bno08x.prodIds.numEntries; n++) {
    Serial.print("Part ");
    Serial.print(bno08x.prodIds.entry[n].swPartNumber);
    Serial.print(": Version :");
    Serial.print(bno08x.prodIds.entry[n].swVersionMajor);
    Serial.print(".");
    Serial.print(bno08x.prodIds.entry[n].swVersionMinor);
    Serial.print(".");
    Serial.print(bno08x.prodIds.entry[n].swVersionPatch);
    Serial.print(" Build ");
    Serial.println(bno08x.prodIds.entry[n].swBuildNumber);
  }

  setReports();

  // Serial.println("Reading events");
  delay(100);
}

void setup(void) {
  Serial.begin(115200);

  setup_gyro();

  pinMode(FAN_OPEN_PIN, INPUT_PULLUP);
  pinMode(FAN_CLOSE_PIN, INPUT_PULLUP);
}

// Here is where you define the sensor outputs you want to receive
void setReports(void) {
  Serial.println("Setting desired reports");
  if (!bno08x.enableReport(SH2_GAME_ROTATION_VECTOR, 10000)) {
    Serial.println("Could not enable game vector");
  }

  // Enabling linear acceleration seems to cause a lot of lag?
  // if (!bno08x.enableReport(SH2_LINEAR_ACCELERATION, 25000)) {
  //   Serial.println("Could not enable linear acceleration");
  // }
}

void writeFloat(float f) {
  byte *b = (byte *)&f;
  Serial.write(b[0]);
  Serial.write(b[1]);
  Serial.write(b[2]);
  Serial.write(b[3]);
  return;
}

float quat_i, quat_j, quat_k, quat_real;
float accel_x, accel_y, accel_z;
char inputBuffer;

void sendState() {
  char switches = 0;
  switches |= fanOpenBtn;
  switches |= fanCloseBtn << 1;
  // Unused
  switches |= 1 << 3;
  switches |= 1 << 4;

  // Send over serial
  Serial.print(switches);
  writeFloat(quat_i);
  writeFloat(quat_j);
  writeFloat(quat_k);
  writeFloat(quat_real);
}

void readGyro() {
  if (bno08x.wasReset()) {
    Serial.print("sensor was reset ");
    setReports();
  }

  if (!bno08x.getSensorEvent(&sensorValue)) {
    return;
  }

  switch (sensorValue.sensorId) {

    case SH2_GAME_ROTATION_VECTOR:
      quat_i = sensorValue.un.gameRotationVector.i;
      quat_j = sensorValue.un.gameRotationVector.j;
      quat_k = sensorValue.un.gameRotationVector.k;
      quat_real = sensorValue.un.gameRotationVector.real;

      // Serial.print("Game Rotation Vector - r: ");
      // Serial.print(sensorValue.un.gameRotationVector.real);
      // Serial.print(" i: ");
      // Serial.print(sensorValue.un.gameRotationVector.i);
      // Serial.print(" j: ");
      // Serial.print(sensorValue.un.gameRotationVector.j);
      // Serial.print(" k: ");
      // Serial.println(sensorValue.un.gameRotationVector.k);
      break;
    case SH2_LINEAR_ACCELERATION:
      accel_x = sensorValue.un.linearAcceleration.x;
      accel_y = sensorValue.un.linearAcceleration.y;
      accel_z = sensorValue.un.linearAcceleration.z;

      // Serial.print("x: ");
      // Serial.print(sensorValue.un.linearAcceleration.x);
      // Serial.print(",y: ");
      // Serial.print(sensorValue.un.linearAcceleration.y);
      // Serial.print(",z: ");
      // Serial.println(sensorValue.un.linearAcceleration.z);
      break;
  }
}

void loop() {
  delay(10);

  readGyro();

  fanOpenBtn = !digitalRead(FAN_OPEN_PIN);
  fanCloseBtn = !digitalRead(FAN_CLOSE_PIN);

  // sendState();
  // Wait for ack from game instance to send state to game instance (non blocking)
  if (Serial.available()) {
    Serial.readBytes(&inputBuffer, 1);
    if (inputBuffer == (char)255) {
      sendState();
    }
  }

  // Serial.print(fanOpenBtn);
  // Serial.print(", ");
  // Serial.println(fanCloseBtn);

  // Serial.println(1);
}