#include "LIB_GT511C3.h"
#include "SoftwareSerial.h"

// Hardware setup - FPS connected to:
//	  digital pin 4(arduino rx, fps tx)
//	  digital pin 5(arduino tx - 560ohm resistor - fps tx - 1000ohm resistor - ground)
//		this voltage divider brings the 5v tx line down to about 3.2v so we dont fry our fps

FPS_GT511C3 fps(4, 5);

void setup()
{
	Serial.begin(9600);
	fps.UseSerialDebug = false; // so you can see the messages in the serial debug screen
	fps.Open();
        fps.SetLED(true);
}


void loop()
{
  if(Serial.available())
  {
    int c = Serial.read();
    
    switch (c)
    {
      case '0':
        Verificar();
      break;
            
      case '1':
        Digitalizar();
      break;
      
      case '2':
        Serial.println(fps.GetEnrollCount());
      break;
      
      case '3':
        fps.DeleteAll();
      break;
    }
  }
}

void Digitalizar()
{
	// Inicio de la digitalizacion

	// Buscar id disponible
	int enrollid = 0;
	bool okid = false;
       
        while(fps.CheckEnrolled(enrollid))
        {
	  if (fps.CheckEnrolled(enrollid)) enrollid++;
          else okid=true;
        }
        
        delay(1000);
	fps.EnrollStart(enrollid);

	// Guardar huella
	Serial.print("Ponga su huella digital: ");
	Serial.println(enrollid);
	while(fps.IsPressFinger() == false) delay(100);
	bool bret = fps.CaptureFinger(true);
	int iret = 0;

	if (bret)
	{
                delay(1000);
		Serial.println("Remueva el dedo");
		fps.Enroll1(); 
		while(fps.IsPressFinger() == true) delay(100);

		Serial.println("Volver a poner su huella");
		while(fps.IsPressFinger() == false) delay(100);
		bret = fps.CaptureFinger(true);

		if (bret)
		{
			Serial.println("Remueva el dedo");
			fps.Enroll2();
			while(fps.IsPressFinger() == true) delay(100);
                        delay(1000);
                        
			Serial.println("Vuelva a poner su huella");
			while(fps.IsPressFinger() == false) delay(100);
			bret = fps.CaptureFinger(true);

			if (bret)
			{
				Serial.println("Remueva el dedo");
				iret = fps.Enroll3();
				if (iret == 0)
				{
					Serial.println("Huella grabada satiscatoriamente");
                                        Serial.println(enrollid);
				}
				else
				{
					Serial.print("Fallo la digitalizacion");
				}
			}
			else Serial.println("Imposible capturar por tercera vez");
		}
		else Serial.println("Imposible capturar por segunda vez");
	}
	else Serial.println("Imposible capturar");
}

void Verificar()
{
   Serial.println("Favor de colocar su huella");
	// Identificar Huella
	if (fps.IsPressFinger())
	{
		fps.CaptureFinger(true);
		int id = fps.Identify1_N();
		if (id <200)
		{
			Serial.print("ID Verfificado:");
			Serial.println(id);
		}
		else
		{
			Serial.println("No se encontro la huella");
                        Serial.println(id);
		}
	}
	else
	{
		Serial.println("Favor de colocar su huella");
	}
	delay(100);
}
