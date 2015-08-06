Servicio de Windows para bloquear memorias USB o USB de tipo STORAGE con C#

El Problema

Son bien conocidos los inconvenientes de MS Windows XP/7 para evitar los virus. En especial, los que utilizan unidades de almacenamiento secundario para propagarse.

Recientemente, he iniciado una migraci�n a Linux por motivos de seguridad (muchos motivos en realidad). Durante el proceso de migraci�n, que suele tomar bastante tiempo (esto depende de muchos factores, claro est�), es necesario mantener a las estaciones operativas.

Es un trabajo sucio que alguien debe hacer, perder un tiempo menor para ganar un tiempo mayor (tiempo que ser� aprovechado para la migraci�n a linux). Ese es el objetivo de este tutorial, establecer pol�ticas efectivas de seguridad para manejar los dispositivos de almacenamiento secundario.

Existen problemas de seguridad esenciales que el sistema operativo deber�a solucionar por si mismo (o en el peor de los casos facilitar las herramientas para que el usuario lo solucionarlo), sin embargo, cuando el sistema operativo no solo no es capaz de establecer pol�ticas de seguridad efectivas sino que tambi�n abre puertas traseras a prop�sito para enviar informaci�n privada (como el spyware de WGA), �qu� tipo de seguridad podr�s esperar?, pues si, ninguna.

La Soluci�n

Como soluci�n moment�nea, se me ocurri� elaborar un servicio que admita solamente los USB STORAGE (unidades de almacenamiento secundario) que se que no tendr�n virus ni traer�n programas de otras partes. Digamos que son dispositivos de almacenamiento de confianza.

Componentes

*Para ello he ideado un filtro simple. Un archivo de texto que llamaremos "usb.b64" encriptado con base64, que va a contener todos los seriales (identificadores �nicos) de los dispositivos, uno por cada l�nea del archivo.

*Un servicio que lee el contenido del archivo "usb.64". Este servicio lee el contenido del archivo solamente cuando sea insertado un dispositivo USB tipo STORAGE.

*Si el dispositivo no est� en la lista (No es un dispositivo de confianza) ser� retirado su hardware inmediatamente y no ser� reconocido nunca de all� en adelante. Para ello usaremos una utilidad de l�nea de comandos llamada "Devcon" que es una aplicaci�n llamada desde nuestro servicio. "Devcon" es algo as� como el administrador de dispositivos desde la consola de MS-DOS. Un ejemplo sencillo en Devcon para desactivar todas las unidades de almacenamiento secundario ser�a:

> devcon disable USBSTOR*

Herramientas que necesitaremos

Antes que nada necesitamos una versi�n del compilador de c# (preferiblemente gratis). No tires tu dinero a la basura comprando la suite completa de Visual Studio a menos que necesites obligatoriamente desarrollar aplicaciones para Windows que utilicen controles especiales propietarios. Yo recomendar�a Microsoft Visual C# 2010 Express

Otra cosa que vamos a necesitar es la utilidad Devcon que les coment� arriba, pueden conseguirla aqu�. Este enlace tambi�n brinda los detalles para utilizar devcon.

C�digo a compilar

El c�digo pudiera ser dif�cil de utilizar o entender ley�ndolo directamente en el blog (si tienes alguna idea puedes hacermela llegar), por eso te invito a copiarlo y pegarlo en tu editor favorito.

Otra cosa que debes conocer es como instalar esta aplicaci�n como servicio. Para ello debes utilizar installutil (Una utilidad que trae el .net framework que suele estar en WINDOWS\Microsoft.NET\Framework\v2.0....). Debes crearle un instalador al servicio