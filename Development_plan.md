360 ViSi Simulation teaching platform

## TGL projektin aloitu meetti
### Konsepti
- Tavoitteena kaksi ohjelmaa (tai saman ohjelman kaksi osaa):
1. Simulaatio player, jolla simulaatiota voi käyttää
2. Simulaatio editor, jolla simulaatioita voi luoda

### Toteutus
Projektia lähdetään tutkimaan vaihe kerrallaan:

1. vaihe:
	- Unitylla toteutetaan toimiva ohjelma, jossa on vähintään kaksi peräkkäistä videota ja videosta toiseen siirrytään "action" valinnan avulla. Valinta mahdollisuuksia on vähintään kaksi, jolloin saadaa haarautuminen kokeiltua
	- Videot toteutetaan 360° videoina
	- Action valinta on videon päälle tulevat napit, joista valinta tehdään
	- Ohjelman pitää toimia VR-headsetissä ja Android puhelimessa

2. vaihe:
	- Videotiedoston latausulkoistetaan, jolloin videon voi vaihtaa ilman Unity editoria.

3. vaihe:
	- Action valinta ulkoistetaan, siten että action valintaan linkitetään "from" ja "to" videot
	- Action valintojen lukumäärä määräytyy samalla ulkoisen tiedoston avulla

4. vaihe:
	- Toteutetaan editori, jolla voi määritellä graafisessa käyttöliittymässä kokonaisen simulaatiovideoiden puun.

5. vaihe:
	- Pohditaan lisää elementtejä, joilla voidaan rikastaa simulaatiota:
		- 360° ympäristössä klikattavia toiminnallisuuksia
		- Alfa kanavallisia video objekteja, joiden ilmestymisen voi ajoittaa videoon
		- Looppaava video, joka tarjoaa action vaihtoehdot 360° ympäristössä
		- 3D malleja
