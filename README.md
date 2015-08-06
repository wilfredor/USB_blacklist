Servicio de Windows para bloquear memorias USB o USB de tipo STORAGE con C#

El Problema

Son bien conocidos los inconvenientes de MS Windows XP/7 para evitar los virus. En especial, los que utilizan unidades de almacenamiento secundario para propagarse.

Recientemente, he iniciado una migración a Linux por motivos de seguridad (muchos motivos en realidad). Durante el proceso de migración, que suele tomar bastante tiempo (esto depende de muchos factores, claro está), es necesario mantener a las estaciones operativas.

Es un trabajo sucio que alguien debe hacer, perder un tiempo menor para ganar un tiempo mayor (tiempo que será aprovechado para la migración a linux). Ese es el objetivo de este tutorial, establecer políticas efectivas de seguridad para manejar los dispositivos de almacenamiento secundario.

Existen problemas de seguridad esenciales que el sistema operativo debería solucionar por si mismo (o en el peor de los casos facilitar las herramientas para que el usuario lo solucionarlo), sin embargo, cuando el sistema operativo no solo no es capaz de establecer políticas de seguridad efectivas sino que también abre puertas traseras a propósito para enviar información privada (como el spyware de WGA), ¿qué tipo de seguridad podrás esperar?, pues si, ninguna.

La Solución

Como solución momentánea, se me ocurrió elaborar un servicio que admita solamente los USB STORAGE (unidades de almacenamiento secundario) que se que no tendrán virus ni traerán programas de otras partes. Digamos que son dispositivos de almacenamiento de confianza.

Componentes

*Para ello he ideado un filtro simple. Un archivo de texto que llamaremos "usb.b64" encriptado con base64, que va a contener todos los seriales (identificadores únicos) de los dispositivos, uno por cada línea del archivo.

*Un servicio que lee el contenido del archivo "usb.64". Este servicio lee el contenido del archivo solamente cuando sea insertado un dispositivo USB tipo STORAGE.

*Si el dispositivo no está en la lista (No es un dispositivo de confianza) será retirado su hardware inmediatamente y no será reconocido nunca de allí en adelante. Para ello usaremos una utilidad de línea de comandos llamada "Devcon" que es una aplicación llamada desde nuestro servicio. "Devcon" es algo así como el administrador de dispositivos desde la consola de MS-DOS. Un ejemplo sencillo en Devcon para desactivar todas las unidades de almacenamiento secundario sería:

> devcon disable USBSTOR*

Herramientas que necesitaremos

Antes que nada necesitamos una versión del compilador de c# (preferiblemente gratis). No tires tu dinero a la basura comprando la suite completa de Visual Studio a menos que necesites obligatoriamente desarrollar aplicaciones para Windows que utilicen controles especiales propietarios. Yo recomendaría Microsoft Visual C# 2010 Express

Otra cosa que vamos a necesitar es la utilidad Devcon que les comenté arriba, pueden conseguirla aquí. Este enlace también brinda los detalles para utilizar devcon.

Código a compilar

El código pudiera ser difícil de utilizar o entender leyéndolo directamente en el blog (si tienes alguna idea puedes hacermela llegar), por eso te invito a copiarlo y pegarlo en tu editor favorito.

Otra cosa que debes conocer es como instalar esta aplicación como servicio. Para ello debes utilizar installutil (Una utilidad que trae el .net framework que suele estar en WINDOWS\Microsoft.NET\Framework\v2.0....). Debes crearle un instalador al servicio