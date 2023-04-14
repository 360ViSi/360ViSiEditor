var krpano;
var jsonList;
var currentVideoNode; //Which element current video is in jsonList 
var currentVideoNodeId; //Videos nodeId written in jsonList
var autoEndNextNodeId;
var currentToolNode;
var currentToolNodeId;
var answerButtonCount = 0;
var screenButtonCount = 0;
var videoHasEnded = false;
var score = 0;
var videosFound = 0;
var toolNodeFilesChecked = 0;
var currentFolderName = "";
const timerArray = [];
const actionShowArray = [];
const actionHideArray = [];
const actionTimeIds = [];
const answerArray = [];

// let myRequest = new Request("./temp/StavangerDemo-v2.json");

// fetch(myRequest)
// 	.then(function(resp){
// 		return resp.json();
// 	})
// 	.then(function(data){
// 		jsonList = data;
// 		console.log(jsonList);
//      console.log("start ID: " + jsonList.startId);
// 	})

function getDemoData(){
    document.getElementById("errorStatus").innerHTML = "Missing files:";
    document.getElementById("errorStatus").style.visibility = "hidden";
    document.getElementById("materialCheckStatus").style.visibility = "hidden";
    document.getElementById("materialCheckStatus2").style.visibility = "hidden";
    document.getElementById("startLabel").style.visibility = "hidden";
    videosFound = 0;
    toolNodeFilesChecked = 0;

    document.getElementById("loadText").innerHTML = "Using demo data";

    let myRequest = new Request("./tours/Postoperative_care_in_recovery_room_4k/Postoperative_careV8-mp4.json");

    fetch(myRequest)
        .then(function(resp){
            return resp.json();
        })
        .then(function(data){
            console.log(data);
            startJsonCheck(data);
        })
}

function importFiles(){
    document.getElementById("errorStatus").innerHTML = "Missing files:";
    document.getElementById("errorStatus").style.visibility = "hidden";
    document.getElementById("materialCheckStatus").style.visibility = "hidden";
    document.getElementById("materialCheckStatus2").style.visibility = "hidden";
    document.getElementById("startLabel").style.visibility = "hidden";
    videosFound = 0;
    toolNodeFilesChecked = 0;
    
    var files = document.getElementById('jsonInput').files[0];
	if (files.length <= 0) {
		return false;
	}
		
	var fr = new FileReader();
		
	fr.onload = function(){
		var fileContent = JSON.parse(fr.result);
		console.log(fileContent);
		startJsonCheck(fileContent);
	};
	fr.readAsText(files);
};

function startJsonCheck(list){
    jsonList = list;
	document.getElementById('import').innerHTML = "Done";
    currentFolderName = "";

    if(!jsonList.folderName || jsonList.folderName == ''){
        document.getElementById("errorStatus").style.visibility = "visible";
        document.getElementById("errorStatus").innerHTML = "Error: Tour folder name not defined, using default folder";
        currentFolderName = "Postoperative_care_in_recovery_room_4k";
    }else{
        currentFolderName = jsonList.folderName;
    }

    document.getElementById("materialCheckStatus").style.visibility = "visible";
    document.getElementById("materialCheckStatus").innerHTML = "Video nodes in order: " + videosFound + "/" + jsonList.videos.length;
    for(var i = 0; i < jsonList.videos.length; i++){
        checkIfVideoExists(i);
    }

    document.getElementById("materialCheckStatus2").style.visibility = "visible";
    document.getElementById("materialCheckStatus2").innerHTML = "Material nodes in order: " + toolNodeFilesChecked + "/" + jsonList.tools.length * 2;
    for(var i = 0; i < jsonList.tools.length; i++){
        checkIfToolFilesExists(i);
    }
};

function updateVideoCheckStatus(){
    videosFound++;
    document.getElementById("materialCheckStatus").innerHTML = "Video nodes in order: " + videosFound + "/" + jsonList.videos.length;

    if(toolNodeFilesChecked == 2 * jsonList.tools.length && videosFound == jsonList.videos.length){
        document.getElementById("startLabel").style.visibility = "visible";
        //document.getElementById("startDebugLabel").style.visibility = "visible";
    }
};

function checkIfVideoExists(node){
    //console.log("Checking video " + node);

    var http = new XMLHttpRequest();
    var src = './tours/' + currentFolderName + '/' + jsonList.videos[node].videoFileName;
    http.open('HEAD', src);

    http.onreadystatechange = function() {
        if (this.readyState == this.DONE) {
            if (this.status != 404) {
                //console.log(this.status);
                //console.log(src + " found");
                updateVideoCheckStatus();
            }
            else{
                document.getElementById("errorStatus").style.visibility = "visible";
                document.getElementById("errorStatus").innerHTML += "<br>" + jsonList.videos[node].videoFileName;
            }
        }
    };

    http.send();
};

function checkIfToolFilesExists(node){
    //console.log("Checking files of tool node " + node);

    if(jsonList.tools[node].spritePath != ""){
        var http = new XMLHttpRequest();
        var src = './tours/' + currentFolderName + '/' + jsonList.tools[node].spritePath;
        http.open('HEAD', src);

        http.onreadystatechange = function() {
            if (this.readyState == this.DONE) {
                if (this.status != 404) {
                    //console.log(src + " found");
                    updateToolNodeCheckStatus();
                }
                else{
                    document.getElementById("errorStatus").style.visibility = "visible";
                    document.getElementById("errorStatus").innerHTML += "<br>" + jsonList.tools[node].spritePath;
                }
            }
        };

        http.send();
    }
    else{
        updateToolNodeCheckStatus();
    }

    if(jsonList.tools[node].video2Dpath != ""){
        var http = new XMLHttpRequest();
        var src = './tours/' + currentFolderName + '/' + jsonList.tools[node].video2Dpath;
        http.open('HEAD', src);

        http.onreadystatechange = function() {
            if (this.readyState == this.DONE) {
                if (this.status != 404) {
                    //console.log(src + " found");
                    updateToolNodeCheckStatus();
                }
                else{
                    document.getElementById("errorStatus").style.visibility = "visible";
                    document.getElementById("errorStatus").innerHTML += "<br>" + jsonList.tools[node].video2Dpath;
                }
            }
        };

        http.send();
    }
    else{
        updateToolNodeCheckStatus();
    }
};

function updateToolNodeCheckStatus(){
    toolNodeFilesChecked++;
    document.getElementById("materialCheckStatus2").innerHTML = "Tool nodes in order: " + toolNodeFilesChecked + "/" + jsonList.tools.length * 2;

    if(toolNodeFilesChecked == 2 * jsonList.tools.length && videosFound == jsonList.videos.length){
        document.getElementById("startLabel").style.visibility = "visible";
        //document.getElementById("startDebugLabel").style.visibility = "visible";
    }
};

function krpanoReady(){
    krpano = document.getElementById("krpanoSWFObject");
    //console.log("krpano is ready");
};

function startSimulation(){
    document.getElementById("upload").style.display = "none";
    // var uploadButtons = document.getElementById("upload");
    // uploadButtons.remove();
    score = 0;
    getNodeData(jsonList.startId);
};

function enableDebugTools(){
    krpano.call("set(layer[layer_video_time].visible, true);");
    krpano.call("set(layer[layer_video_time_percent].visible, true);");
    krpano.call("set(layer[layer_scene_time].visible, true);");
}

function print(s){
    console.log(s);
};

function getNodeData(id){
    //print(id);
    if(id == -1){
        endSimulation();
    }
    for (var i = 0; i < jsonList.videos.length; i++){
        if (jsonList.videos[i].nodeId == id){
            //console.log("clicked video node # " + i + "\nnode id: " + id);
            cancelActionVisibilityCalls();
            sceneChangeInProgress = 1;
            currentVideoNode = i;
            currentVideoNodeId = id;
            openVideoNode(currentVideoNode);
        }
    }
    for (var j = 0; j < jsonList.tools.length; j++){
        if (jsonList.tools[j].nodeId == id){
            //console.log("clicked tool node # " + j + "\nnode id: " + id);
            currentToolNode = j;
            currentToolNodeId = id;
            openQuestionSheet();
        }
    }
};

function openVideoNode(element){
    resetTimer();
    resetTimerArray();
    screenButtonCount = 0;
    videoHasEnded = false;
    var horizontalRotation = jsonList.videos[element].videoStartRotation.y - 90;
    var startTime = jsonList.videos[element].startTime * 100;
    startTime = startTime + "%";
    var videoFileNameCall = "newVideoScene(" + element + ", 'tours/"+ currentFolderName +"/" + jsonList.videos[element].videoFileName + "', " + horizontalRotation + ", " 
    + jsonList.videos[element].videoStartRotation.x + ", " + startTime + ", " + jsonList.videos[element].endTime + ");";
    console.log(videoFileNameCall);
    krpano.call(videoFileNameCall);
};

function videoEnded(id){
    //console.log("called video end, with id: " + id + ", current video node is: " + currentVideoNode);
    if(id != currentVideoNode){
        return;
    }
    if(jsonList.videos[currentVideoNode].loop == true){
        krpano.call("plugin[video].seek(get(plugin[video].starttime));");
        krpano.call("set(plugin[video].timepercent, 0);");
        krpano.call("callwhen(plugin[video].time GE plugin[video].endtimeseconds, js(videoEnded(" + currentVideoNode + ")));");
        krpano.call("plugin[video].play();");

        callActionVisibilityCalls();
    }
    else{
        krpano.call("plugin[video].pause();");
        videoHasEnded = true;
    }
    if(autoEndNextNodeId != -2){
        getNodeData(autoEndNextNodeId);
    }
};

function videoPaused(){
    if(videoHasEnded) return;

    pauseTimer();

    krpano.call("set(layer[pause_layer].enabled, true);");
    krpano.call("set(layer[pause_layer].visible, true);");
};

function unPauseVideo(){
    if(videoHasEnded) return;

    krpano.call("plugin[video].play();");

    krpano.call("set(layer[pause_layer].enabled, false);");
    krpano.call("set(layer[pause_layer].visible, false);");
}

function createHotspots(){
    //console.log("This scene has " + jsonList.videos[currentVideoNode].actions.length + " actions.");
    autoEndNextNodeId = -2;
    actionShowArray.length = 0;
    actionHideArray.length = 0;
    actionTimeIds.length = 0;
    for(var i = 0; i < jsonList.videos[currentVideoNode].actions.length; i++){
        if(jsonList.videos[currentVideoNode].actions[i].actionType == 4){
            // var timerId = "timer" + currentVideoNode + "_" + i;
            // console.log(timerId);
            // timerArray.push(timerId);
            // var timerActionCall = "delayedcall(" + timerId + ", " + jsonList.videos[currentVideoNode].actions[i].timer + ", js(getNodeData(" + 
            // jsonList.videos[currentVideoNode].actions[i].nextNode + ")));";
            // console.log(timerActionCall);
            // krpano.call(timerActionCall);

             var timerActionCall = "js(getNodeData(" + jsonList.videos[currentVideoNode].actions[i].nextNode + "));";
             createNewTimer(jsonList.videos[currentVideoNode].actions[i].timer, timerActionCall);
        }
        else{
            if(jsonList.videos[currentVideoNode].actions[i].autoEnd == true){
                autoEndNextNodeId = jsonList.videos[currentVideoNode].actions[i].nextNode;
                //console.log("this scene will end automatically and go to node: " + autoEndNextNodeId);
            }
            else{
                if(jsonList.videos[currentVideoNode].actions[i].worldPosition.x == 0 && jsonList.videos[currentVideoNode].actions[i].worldPosition.y == 0 && jsonList.videos[currentVideoNode].actions[i].worldPosition.z == 0){
                    var layerCreationCall = "createLayer(" + currentVideoNode + i + ", " + screenButtonCount + ", " + jsonList.videos[currentVideoNode].actions[i].actionText + ", " + jsonList.videos[currentVideoNode].actions[i].nextNode + ")";
                    krpano.call(layerCreationCall);
                    //print(layerCreationCall);
                    screenButtonCount++;

                    var layerCallWhenId1 = "lashow" + currentVideoNode + i;
                    var layerCallWhenId2 = "lahide" + currentVideoNode + i;
                    var layerShowCall = "callwhen(" + layerCallWhenId1 + ", plugin[video].timepercent GE " + jsonList.videos[currentVideoNode].actions[i].startTime + ", set(layer[layer" + currentVideoNode + i + "].visible, true); set(layer[layer" + currentVideoNode + i + "].enabled, true););";
                    var layerHideCall = "callwhen(" + layerCallWhenId2 + ", plugin[video].timepercent GE " + jsonList.videos[currentVideoNode].actions[i].endTime + ", set(layer[layer" + currentVideoNode + i + "].visible, false); set(layer[layer" + currentVideoNode + i + "].enabled, false););";
                    actionShowArray.push(layerShowCall);
                    actionHideArray.push(layerHideCall);
                    actionTimeIds.push(layerCallWhenId1);
                    actionTimeIds.push(layerCallWhenId2);
                } 
                else {
                    var hotspotCreationCall = "createHotspot(" + currentVideoNode + i + ", " + getActionAth(i) + ", " + getActionAtv(i) + ", " + 
                    jsonList.videos[currentVideoNode].actions[i].nextNode + ", " + getActionNodeImage(i) + ");";
                    //console.log(hotspotCreationCall);
                    krpano.call(hotspotCreationCall);

                    var labelAtv = getActionAtv(i) - 4;
                    var hotspotLabelCreationCall = "createLabel(" + currentVideoNode + i + ", " + getActionAth(i) + ", " + labelAtv + ", '" + 
                    jsonList.videos[currentVideoNode].actions[i].actionText + "');" ;
                    //console.log(hotspotLabelCreationCall);
                    krpano.call(hotspotLabelCreationCall);

                    var hotspotCallWhenId1 = "hsshow" + currentVideoNode + i;
                    var hotspotCallWhenId2 = "hshide" + currentVideoNode + i;
                    var hotspotShowCall = "callwhen(" + hotspotCallWhenId1 + ", plugin[video].timepercent GE " + jsonList.videos[currentVideoNode].actions[i].startTime + ", set(hotspot[hotspot" + currentVideoNode + i + "].visible, true); set(hotspot[hotspot" + currentVideoNode + i + "].enabled, true););";
                    var hotspotHideCall = "callwhen(" + hotspotCallWhenId2 + ", plugin[video].timepercent GE " + jsonList.videos[currentVideoNode].actions[i].endTime + ", set(hotspot[hotspot" + currentVideoNode + i + "].visible, false); set(hotspot[hotspot" + currentVideoNode + i + "].enabled, false););";
                    actionShowArray.push(hotspotShowCall);
                    actionHideArray.push(hotspotHideCall);
                    actionTimeIds.push(hotspotCallWhenId1);
                    actionTimeIds.push(hotspotCallWhenId2);
                }
            }
        }
    }

    callActionVisibilityCalls();
};

function callActionVisibilityCalls(){
    if(actionShowArray.length > 0){
        actionShowArray.forEach(string => krpanoCall(string));
    }

    if(actionHideArray.length > 0){
        actionHideArray.forEach(string => krpanoCall(string));
    }
};

//Cancel delayed calls that were created in previous scene
function cancelActionVisibilityCalls(){
    if(actionTimeIds.length > 0){
        actionTimeIds.forEach(string => krpanoCall("stopcallwhen(" + string + ")"));
    }
};

function krpanoCall(call){
    // console.log(call);
    krpano.call(call);
};

function resetTimerArray(){
    if(timerArray.length > 0){
        // console.log(timerArray);
        //timerArray.forEach(stopDelayedCall);
        timerArray.length = 0;
        // console.log(timerArray);
    }
};

function stopDelayedCall(id){
    var cancelCall = "stopdelayedcall(" + id + ");";
    // console.log(cancelCall);
    krpano.call(cancelCall);
};

function getActionNodeImage(id){
    var imageUrl = "sprites/icon_default.png";
    if(jsonList.videos[currentVideoNode].actions[id].iconName == "moving"){
        imageUrl = "sprites/icon_walk.png";
    }
    else if(jsonList.videos[currentVideoNode].actions[id].iconName == "touch"){
        imageUrl = "sprites/icon_hand.png";
    }
    else if(jsonList.videos[currentVideoNode].actions[id].iconName == "speak"){
        imageUrl = "sprites/icon_speak.png";
    }
    return imageUrl;
};

function getActionAth(id){
    var ath;

    if(jsonList.videos[currentVideoNode].actions[id].worldPosition.x < 0 && jsonList.videos[currentVideoNode].actions[id].worldPosition.z >= 0){
        ath = -(180 + (180/Math.PI) * Math.atan(jsonList.videos[currentVideoNode].actions[id].worldPosition.z / jsonList.videos[currentVideoNode].actions[id].worldPosition.x));
    }
    else if(jsonList.videos[currentVideoNode].actions[id].worldPosition.x >= 0 && jsonList.videos[currentVideoNode].actions[id].worldPosition.z >= 0){
        ath = -(180/Math.PI) * Math.atan(jsonList.videos[currentVideoNode].actions[id].worldPosition.z / jsonList.videos[currentVideoNode].actions[id].worldPosition.x);
    }
    else if(jsonList.videos[currentVideoNode].actions[id].worldPosition.x < 0 && jsonList.videos[currentVideoNode].actions[id].worldPosition.z < 0){
        ath = 180 - (180/Math.PI) * Math.atan(jsonList.videos[currentVideoNode].actions[id].worldPosition.z / jsonList.videos[currentVideoNode].actions[id].worldPosition.x);
    }
    else if(jsonList.videos[currentVideoNode].actions[id].worldPosition.x >= 0 && jsonList.videos[currentVideoNode].actions[id].worldPosition.z < 0){
        ath = -(180/Math.PI) * Math.atan(jsonList.videos[currentVideoNode].actions[id].worldPosition.z / jsonList.videos[currentVideoNode].actions[id].worldPosition.x);
    }

    return ath;
};

function getActionAtv(id){

    var atv;

    atv = (180/Math.PI) * Math.atan(jsonList.videos[currentVideoNode].actions[id].worldPosition.y / Math.sqrt(Math.pow(jsonList.videos[currentVideoNode].actions[id].worldPosition.z, 2) + Math.pow(jsonList.videos[currentVideoNode].actions[id].worldPosition.x, 2)))

    return -atv;
};

function openQuestionSheet(){
    if(jsonList.tools[currentToolNode].toolTypeInt == 0){
        var randomNode = jsonList.tools[currentToolNode].nextNodes[Math.floor(Math.random() * jsonList.tools[currentToolNode].nextNodes.length)];

        hideAnswerButtons();
        krpano.call("set(layer[question_sheet].visible, false);");
        krpano.call("set(layer[question_sheet].enabled, false);");

        getNodeData(randomNode);

        return;
    }

    pauseTimer();

    answerArray.length = 0;

    krpano.call("plugin[video].pause();");
    
    //var popUpContent = getHtmlContent();
    var popUpContent = "empty";

    var closeButtonBool = false;

    if(jsonList.tools[currentToolNode].toolTypeInt == 2){
        closeButtonBool = true;
    }

    //krpano.call("popup('html', " + popUpContent + ", get(windowsizes.size[popup].width), get(windowsizes.size[popup].height), true);");
    krpano.call("popup('html', " + popUpContent + ", " + closeButtonBool + ");");
}

function getHtmlContent(){
    var htmlContent = "";

    if(jsonList.tools[currentToolNode].toolTypeInt == 2){
        if(jsonList.tools[currentToolNode].spritePath != ""){
            htmlContent += '<img src="tours/' + currentFolderName +'/' + jsonList.tools[currentToolNode].spritePath + '" alt="' + jsonList.tools[currentToolNode].infoText + '" class="info-window-material">';
        }
        if(jsonList.tools[currentToolNode].video2Dpath != ""){
            htmlContent += '<video class="info-window-material" controls>';
            htmlContent += '<source src="tours/' + currentFolderName + '/' + jsonList.tools[currentToolNode].video2Dpath + '" type="video/mp4">';
            htmlContent += '</video>';
        }
        if(jsonList.tools[currentToolNode].infoText != ""){
            //console.log(jsonList.tools[currentToolNode].infoText);
            htmlContent += "<p>" + jsonList.tools[currentToolNode].infoText + "</p>";
        }
    }
    else if(jsonList.tools[currentToolNode].toolTypeInt == 1){
        htmlContent = "<h1>" + jsonList.tools[currentToolNode].question.questionTitleText + "</h1>";
        //console.log(jsonList.tools[currentToolNode].question.questionText);
        htmlContent += "<p>" + jsonList.tools[currentToolNode].question.questionText + "</p><br>";

        if(jsonList.tools[currentToolNode].spritePath != ""){
            htmlContent += '<img src="tours/' + currentFolderName +'/' + jsonList.tools[currentToolNode].spritePath + '" alt="' + jsonList.tools[currentToolNode].infoText + '" class="info-window-material"><br>';
        }
        if(jsonList.tools[currentToolNode].video2Dpath != ""){
            htmlContent += '<video class="info-window-material" controls>';
            htmlContent += '<source src="tours/' + currentFolderName + '/' + jsonList.tools[currentToolNode].video2Dpath + '" type="video/mp4">';
            htmlContent += '</video>';
        }
        if(jsonList.tools[currentToolNode].infoText != ""){
            htmlContent += "<p>" + jsonList.tools[currentToolNode].infoText + "</p>";
        }

        if(jsonList.tools[currentToolNode].question.answers.length > 0){
            if(jsonList.tools[currentToolNode].question.multichoice == false){
                for(let i=0; i < jsonList.tools[currentToolNode].question.answers.length; i++){
                    htmlContent += '<button type="button" class="button" onclick="nodeAfterAnswer(' + jsonList.tools[currentToolNode].question.correctAnswers[i] + ')">' + jsonList.tools[currentToolNode].question.answers[i] + '</button><br>';
                }
            }
            else{
                for(let i=0; i < jsonList.tools[currentToolNode].question.answers.length; i++){
                    htmlContent += 
                    '<label class="container">' +
                        '<input type="checkbox" id="box'+ i +'" onclick="enableSubmitButton();">' +
                        '<span class="checkmark"></span>' + jsonList.tools[currentToolNode].question.answers[i] +
                    '</label><br>';
                }

                htmlContent += '<br><button id="submitButton" type="button" class="button" onclick="checkAnswers()" disabled>Submit</button>';
            }
        }
    }

    //console.log(htmlContent);
    return htmlContent;
}

function enableSubmitButton(){
    document.getElementById("submitButton").disabled = false;
}

//Old question sheet call. No Use for now
function openQuestionSheetOld(){
    pauseTimer();

    answerArray.length = 0;

    if(answerButtonCount > 0){
        for(var i = 0; i < answerButtonCount; i++){
            var answerButtonEraseCall = "removelayer(answerbutton" + i + ");";
            krpano.call(answerButtonEraseCall);
        }
        answerButtonCount = 0;
    }

    krpano.call("plugin[video].pause();");
    krpano.call("set(control.usercontrol, off);");
    krpano.call("set(layer[question_sheet].visible, true);");
    krpano.call("set(layer[question_sheet].enabled, true);");
    krpano.call("set(layer[submit_button].bgcolor, '0xBCBCBC');");
    krpano.call("set(layer[close_button].bgcolor, '0xBCBCBC');");

    var titleCall = "set(layer[question_title].html, '" + jsonList.tools[currentToolNode].question.questionTitleText + "');";
    var textCall = "set(layer[question_text].html, '" + jsonList.tools[currentToolNode].question.questionText + "');";

    if(jsonList.tools[currentToolNode].toolTypeInt == 2){
        var infoText = jsonList.tools[currentToolNode].infoText;
        var infoTextFix = infoText.replace(/\r\n/g, "[br]");
        var infoCall1 = "set(layer[info_text_container].html, '" + infoTextFix + "');";
        var infoCall2 = "set(layer[info_text_container].visible, true);"
        var infoCall3 = "set(layer[info_text_container].enabled, true);"
        var infoCall4 = "set(layer[info_text].html, '');";

        krpano.call(infoCall1);
        krpano.call(infoCall2);
        krpano.call(infoCall3);
        krpano.call(infoCall4);
    }
    else{
        var infoCall1 = "set(layer[info_text].html, '" + jsonList.tools[currentToolNode].infoText + "');";
        var infoCall2 = "set(layer[info_text_container].visible, false);"
        var infoCall3 = "set(layer[info_text_container].enabled, false);"

        krpano.call(infoCall1);
        krpano.call(infoCall2);
        krpano.call(infoCall3);
    }

    krpano.call(titleCall);
    krpano.call(textCall);

    if(jsonList.tools[currentToolNode].question.answers.length > 0){
        var answerContainerHeight = jsonList.tools[currentToolNode].question.answers.length * 50;
        // console.log("Container height set to: " + answerContainerHeight);
        var answerContainerCall = "set(layer[answer_container].height, " + answerContainerHeight +" );";

        krpano.call(answerContainerCall);

        // console.log("Multichoice value: " + jsonList.tools[currentToolNode].question.multichoice);

        if(jsonList.tools[currentToolNode].question.multichoice == false)
        {
            for(var i=0; i < jsonList.tools[currentToolNode].question.answers.length; i++){
                var buttonCall1 = "addlayer(answerbutton" + i + ");";
                var buttonCall2 = "set(layer[answerbutton" + i + "].parent, layer[answer_container]);";
                var buttonCall21 = "assignstyle(layer[answerbutton" + i + "], 'answerbutton');";
                var buttonCall22 = "set(layer[answerbutton" + i + "].y, " + i * 50 + ");";
                var buttonCall3 = "set(layer[answerbutton" + i + "].html, '" + jsonList.tools[currentToolNode].question.answers[i] + "');";
                var buttonCall4 = "set(layer[answerbutton" + i + "].onclick, js(nodeAfterAnswer(" + jsonList.tools[currentToolNode].question.correctAnswers[i] + ")));";
                // console.log(buttonCall22);
                // console.log(buttonCall4);

                krpano.call(buttonCall1);
                krpano.call(buttonCall2);
                krpano.call(buttonCall21);
                krpano.call(buttonCall22);
                krpano.call(buttonCall3);
                krpano.call(buttonCall4);

                answerButtonCount = i + 1;
            }
        }
        else{
            //console.log("This is multichoice question");
            krpano.call("set(layer[submit_button].visible, true);");
            krpano.call("set(layer[submit_button].enabled, true);");

            for(var i=0; i < jsonList.tools[currentToolNode].question.answers.length; i++){
                var buttonCall1 = "addlayer(answerbutton" + i + ");";
                var buttonCall2 = "set(layer[answerbutton" + i + "].parent, layer[answer_container]);";
                var buttonCall21 = "assignstyle(layer[answerbutton" + i + "], 'answerbutton');";
                var buttonCall22 = "set(layer[answerbutton" + i + "].y, " + i * 50 + ");";
                var buttonCall23 = "set(layer[answerbutton" + i + "].selected, false);";
                var buttonCall3 = "set(layer[answerbutton" + i + "].html, '" + jsonList.tools[currentToolNode].question.answers[i] + "');";
                var buttonCall4 = "set(layer[answerbutton" + i + "].onclick, toggleAnswer(answerbutton" + i + ", " + i + "));";
                //console.log(buttonCall22);
                //console.log(buttonCall4);

                krpano.call(buttonCall1);
                krpano.call(buttonCall2);
                krpano.call(buttonCall21);
                krpano.call(buttonCall22);
                krpano.call(buttonCall23);
                krpano.call(buttonCall3);
                krpano.call(buttonCall4);

                answerButtonCount = i + 1;

                answerArray.push(1);
            }

            //console.log(answerArray);
        }
    }

    if(jsonList.tools[currentToolNode].toolTypeInt == 2){
        krpano.call("set(layer[answer_mask].visible, false);");
        krpano.call("set(layer[answercontainer_slider_bg].visible, false);");
    }
    else{
        krpano.call("set(layer[answer_mask].visible, true);");
        krpano.call("delayedcall(0.1, toggleAnswerSlider());");
    }
};

function changeAnswer(elem, value){
    answerArray[elem] = parseInt(value);
};

function submitAnswers(){
    var nextNode = 0;
    for(var i = 0; i < jsonList.tools[currentToolNode].question.correctAnswers.length; i++){
        if(!answerArray[i] == jsonList.tools[currentToolNode].question.correctAnswers[i]){
            console.log("Answer " + i + " is wrong");
            nextNode = 1;
        }
        else{
            console.log("Answer " + i + " is correct");
        }
        if(answerArray[i] == 0){
            console.log(i + " was chosen answer and it gives " + jsonList.tools[currentToolNode].question.answerScores[i] + " points");
            score += jsonList.tools[currentToolNode].question.answerScores[i];
        }
    }

    console.log("current score is " + score);
    getNodeData(jsonList.tools[currentToolNode].nextNodes[nextNode]);
};

function checkAnswers(){
    var nextNode = 0;
    for(var i = 0; i < jsonList.tools[currentToolNode].question.correctAnswers.length; i++){
        var boxToCheck = "box" + i;
        if(document.getElementById(boxToCheck).checked){
            if(!jsonList.tools[currentToolNode].question.correctAnswers[i] == 0){
                nextNode = 1;
            }
            score += jsonList.tools[currentToolNode].question.answerScores[i];

        } else{
            if(!jsonList.tools[currentToolNode].question.correctAnswers[i] == 1){
                nextNode = 1;
            }
        }
    }

    console.log("current score is " + score);
    getNodeData(jsonList.tools[currentToolNode].nextNodes[nextNode]);

}

function nodeAfterAnswer(id){
    hideAnswerButtons();
    krpano.call("set(layer[question_sheet].visible, false);");
    krpano.call("set(layer[question_sheet].enabled, false);");

    var nodeAfterId = jsonList.tools[currentToolNode].nextNodes[id];
    getNodeData(nodeAfterId);
};

function hideAnswerButtons(){
    for(var i=0; i < 15; i++){
        var j = i + 1;
        var buttonCall1 = "set(layer[answer_button" + j + "].visible, false);";
        var buttonCall2 = "set(layer[answer_button" + j + "].enabled, false);";

        krpano.call(buttonCall1);
        krpano.call(buttonCall2);
    };
};

function endSimulation(){
    print("Finished");
    var scoreText = "Total score: " + score;
    krpano.call("endSimulation(" + scoreText + ");");
    resetTimer();
    resetTimerArray();
    pauseTimer();
    screenButtonCount = 0;


};

//------------------------------------------------------------------------------------------------------
//Timer

function createNewTimer(time, call){
    const t = {};
    t.time = time;
    t.call = call;
    t.called = 0;
    timerArray.push(t);
};

var seconds = 00;
var tens = 00;
var stringseconds = "00";
var stringtens = "00";
var Interval;

function startTimer(){
    clearInterval(Interval);
    Interval = setInterval(runTimer, 10);
};

function pauseTimer(){
    clearInterval(Interval)
};

function resetTimer(){
    clearInterval(Interval);
    tens = 00;
  	seconds = 00;
};

function runTimer () {
    tens++; 
    
    if(tens <= 9){
      stringtens = "0" + tens;
    }
    
    if (tens > 9){
      stringtens = tens;
      
    } 
    
    if (tens > 99) {
      seconds++;
      timerArray.forEach(element => checkTimerCall(element));
      stringseconds = "0" + seconds;
      tens = 0;
      stringtens = "0" + 0;
    }
    
    if (seconds > 9){
      stringseconds = seconds;
    }

    var krpanotime = stringseconds + ":" + stringtens;
    krpano.call("set(layer[layer_scene_time].html, '" + krpanotime + "');");
  
};

function checkTimerCall(t){
    if(t.time <= seconds && t.called == 0){
        krpano.call(t.call);
        t.called = 1;
    }
}
