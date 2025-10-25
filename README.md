## Sovelluksen kuvaus



Sovellus on yksinkertainen viestisovellus, jossa käyttäjät voivat lähettää julkisia viestejä, sekä toiselle käyttäjälle kohdennettuja yksityisiä viestejä. Sovellus toimii periaatteella: jos viestillä ei ole vastaanottajaa, se on julkinen ja näkyy kaikille käyttäjille.



## Sovelluksen tarjoamat rajapinnat



GET: api/Messages
- Palauttaa 10 uusinta julkista viestiä.



GET: api/Message/{id}
- Palauttaa annetulla id:llä olevan viestin, jos se on julkinen tai käyttäjä on viestin lähettäjä tai vastaanottaja.



PUT: api/Messages/{id}
- Päivittää annetulla id:llä olevan viestin, jos käyttäjä on viestin lähettäjä.
- Päivityksen onnistuessa palauttaa http viestin 204 No Content



POST: api/Messages
- Luo uuden viestin, asettaen kirjautuneena olevan käyttäjän viestin lähettäjäksi.
- Luomisen onnistuessa palauttaa kopion tallennetusta viestistä.



DELETE: api/Messages/{id}
- Poistaa annetulla id:llä olevan käyttäjän, jos käyttäjä on kirjautuneena (eli jos poistettava käyttäjä on käyttäjän oma käyttäjä).
- Poistamisen onnistuessa palauttaa http viestin 204 No Content



GET: api/Users
- Palauttaa 10 uusinta käyttäjää (Käyttäjistä ei palauteta kuin käyttäjänimi, etu- ja sukunimet sekä käyttäjän luomisen ja viimeisimmän kirjautumisen aikaleimat).



GET: api/Users/{id}
- Palauttaa annetulla id:llä olevan käyttäjän.



PUT: api/Users/{id}
- Päivittää anneetulla id:llä olevan käyttäjän käyttäjätietoja, jos päivitettävä käyttäjä on käyttäjän oma käyttäjä.
- Päivityksen onnistuessa palauttaa http viestin 204 No Content



POST: api/Users
- Luo uuden käyttäjän annetuilla tiedoilla, mahdollista tehdä ilman tunnistautumista tietämällä api key:n
- Luomisen onnistuessa palauttaa kopion tallennetusta käyttäjästä



DELETE: api/Users/{id}
- Poistaa annetulla id:llä olevan käyttäjän jos käyttäjä on käyttäjän oma käyttäjä.
- Poistamisen onnistuessa palauttaa http viestin 204 No Content



Poislukien uuden käyttäjän luomiseen käytettävän "POST: api/Users", kaikkien muiden rajapintojen käyttämiseen tarvitaan käyttäjän tunnistus ja oikea API key. (Uuden käyttäjän luomiseenkin tarvitaan api key) Näin uusi käyttäjä voi aloittaa sovelluksen käytön itsenäisesti luomalla ensin käyttäjätunnuksen, jolla kirjautumalla pääsee käyttämään sovelluksen muita rajapintoja.



## Sovelluksen rakenne



Sovellus koostuu kolmikerrosrakenteesta: Controllerit, Servicet ja Repositoryt. 

- Controllerit vastaavat http viestien vastaanottamisesta ja niiden ohjaamisesta oikealle funktiolle.

- Servicet vastaavat tietojen tarkastuksesta ja tarvittavista toiminnallisuuksista.

- Repositoryt vastaavat tietokannan käytöstä.



Näiden lisäksi on kansiorakenteesta löytyy kansiot: Middleware, Models ja Migrations.

- Middleware kansiossa on funktiot ApiKey:n oikeellisuuden tarkastamiseen sekä käyttäjän tunnistamiseen ja salasanatiivisteen luomiseen.

- Models kansiossa on malliluokat, joiden perusteella tietokanta on luotu sekä DTO malliluokat

- Migrations kansioon luotavien migraatioiden avulla voidaan tehdä muutoksia tietokantaan. Ensin luodaan uusi migraatio ja sen jälkeen viedään se tietokantaan, jolloin muutokset tulevat voimaan.



DTO:t eli Data transfer objectit

- Sovelluksen eri tasoilla käytetään erilaista oliota sovelluksessa käytettävistä tiedoista. ESIM. Controller tasolla käytetään MessageDTO oliota, joka muutetaan Service tasolla Message olioksi ennen sen siirtämistä Repository tasolle. Data transfer objectien tarkoituksena on vähentää verkon yli siirrettävän tiedon määrä ja siirtää ainoastaan tarvittavat tiedot tietokannasta käyttäjälle. ESIM. Jos haetaan tietty käyttäjä ei tarvitse eikä kuulu siirtää verkon yli kyseisen käyttäjän salasanaa, joka kuitenkin tulee tietokannasta repository tasolla. Tämän vuoksi olio muutetaan DTOksi, jolloin siitä otetaan talteen ainoastaan tarvittavat tiedot ja siirretään eteenpäin.



## ESIMERKKI SOVELLUKSEN TOIMINNASTA



Kun käyttäjä haluaa luoda uuden viestin ja kutsuu rajapintaa POST: api/Messages toimii sovellus seuraavasti:



1\. Viesti saapuu ja ApiKeyMiddleware tarkastaa, että kutsun headers osiossa on oikea ApiKey



2\. Jos ApiKey on oikea MessagesController ottaa viestin vastaan



3\. Koska MessagesControllerilla on attribuutti "\[Authorize]" tarkistetaan ensin, että käyttäjä on kirjautunut sisään kutsumalla funktiota BasicAuthenticationHandler



4\. BasicAuthenticationHandler etsii kutsun headers osiosta kentän "Authorization", josta löytyy käyttäjätunnus ja salasana base64 koodattuna. Funktio muuttaa ne normaalitekstiksi ja tarkistaa, että annetun käyttäjän salasana on annettu salasana (eli tarkistaa, että kirjautumistiedot ovat oikein). Tarkistus tehdään tekemällä salasanasta uusi salasanatiiviste ja vertaamalla sitä tietokantaan tallennettuun tiivisteeseen.



5\. Kun käyttäjän tunnistautuminen on varmistettu MessagesController asettaa MessageDTO olion lähettäjä kenttään viestin lähettäjäksi käyttäjän jolla kirjautuneena viesti on lähetetty ja kutsuu MessageServicen funktiota CreateMessageAsync ja antaa parametrinä messageDTO olion.



6\. MessageServicen CreateMessageAsync funktio kutsuu MessageRepositoryn funktiota CreateMessageAsync. Ensin kuitenkin MessageDTO olio muunnetaan Message olioksi, jonka jälkeen se annetaan parametriksi MessageRepositoryn CreateMessageAsync funktiolle.



7\. MessageRepositoryn funktio CreateMessageAsync tallentaa viestin tietokantaan ja palauttaa Message olion.



8\. MessageServicen CreateMessageAsync funktio muuttaa palautetun Message olion MessageDTO olioksi ja palauttaa sen.



9\. MessageController tallentaa MessageServicen palauttaman MessageDTO olion muuttujaan ja kaiken onnistuessa palauttaa http viestin 201 Created ja viestin rungossa tallennetun viestin sisällön.

