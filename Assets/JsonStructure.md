# JSON Structure: #

- Json rakenne, jonka editori tallentaa

## "Root": ##

- Video, toolit sisältävät kaikki erilliset nodet
- startId määrittää millä nodeIdllä simulaatio alkaa
- EDITOR: startNodePosition ja endNodePosition

json:

    "videos": [],
    "tools": [],
    "startId": 0,
    "startNodePosition": { "x": -27.374652862548829, "y": 158.67283630371095},
    "endNodePosition": { "x": -27.374652862548829, "y": 158.67283630371095}

## Video: ##

- nodeId on uniikki id jokaiselle nodelle (video/tool)
- startTime, endTime ja loopTime ovat lukuja 0 - 1, suhteessa videon pituuteen
- startTime ja endTime kertovat videon leikkauksen
- loopTime on aika johon video menee jos loop on päällä ja video menee loppuun
- EDITOR: nodePosition

json:

    "nodeId": 0,
    "videoFileName": "start_video.webm",
    "startTime": 0.0,
    "endTime": 1.0,
    "loop": true,
    "loopTime": 0.0,
    "videoStartRotation": {
        "x": 359.7035827636719,
        "y": 54.50209045410156,
        "z": 0.0
    },
    "nodePosition": {
        "x": 69.84043884277344,
        "y": 24.296554565429689
    },
    "actions": []

## Tool: ##

- Toolit ovat kaikki muut kuin video tai action nodet 
- Vaihtoehtoja tällä hetkellä: Random, Question ja Info, joka määräytyy toolTypeInt muuttujalla
- Random valitsee nextNodes listasta satunnaisen vaihtoehdon ja vaihtaa suoraan siihen
- Question esittää kysymyksen (questionText) ja vastaavat vastausvaihtoehdot
    - jos multipleChoice on true, vaihtoehtoja oikeita vaihtoehtoja voi olla monta
- Info käyttää infoTextiä ja näyttää sen ruudulle kun kyseiseen nodeen tullaan
    - Infossa ei ole tarvettta edes siirtyä seuraavaan nodeen ja nextNodes voikin olla tyhjä

json:

    "nodeId": 3,
    "nextNodes": [
        -1,
        0
    ],
    "nodePosition": {
        "x": 271.48260498046877,
        "y": 194.50318908691407
    },
    "toolTypeInt": 1,
    "question": {
        "multipleChoice": false,
        "questionText": "Question #1",
        "answers": [
            "Answer #1",
            "Answer #2"
        ],
        "correctAnswers": [
            1
        ]
    },
    "infoText": ""

## Action: ##

- Actionit ovat kaikki interaktiot videoissa
- Tällähetkellä implementoituja actioneitä:
    - ScreenButton, käyttöliittymä näppäin ruudulla
    - WorldButton, nappula pelimaailmassa, jossa ikoni
    - FloorButton, nappula pelimaailmassa, joka on käännetty lattiaan sopivasti
    - AreaButton, nappula, jonka kulmat määritellään areaMarkerVertices listalla
- actionText on teksti joka voi olla nappuloissa tai niide päällä, riippuen actionin tyypistä
- nextNode määrittää mihin nodeen action vie
- autoEnd on 
- startTime ja endTime määrittävät koska action on näkyvissä videolla
- worldPosition määrittää missä action on pelimaailmassa
- iconName on ikoni, joka on valittu, käytössä vain 

json:

    "actionText": "Info",
    "nextNode": 1,
    "autoEnd": false,
    "startTime": 0.0,
    "endTime": 1.0,
    "actionType": 3,
    "worldPosition": {
        "x": -7.265567779541016,
        "y": -0.37451428174972536,
        "z": 99.24755096435547
    },
    "iconName": "hand",
    "areaMarkerVertices": [
        {
            "x": 12.896551132202149,
            "y": -31.55912971496582,
            "z": 6.086055755615234
        },
        {
            "x": -10.774182319641114,
            "y": -26.65019989013672,
            "z": 4.5182929039001469
        },
        {
            "x": -9.246687889099121,
            "y": 26.07317352294922,
            "z": 4.056175231933594
        },
        {
            "x": 12.685699462890625,
            "y": 30.893835067749025,
            "z": 5.8498616218566898
        }
    ]