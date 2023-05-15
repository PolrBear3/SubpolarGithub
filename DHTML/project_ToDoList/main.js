let taskInput = document.getElementById("task-input");
let addButton = document.getElementById("add-button");
let allTasks = [];

addButton.addEventListener("click", Add_New_Task);

function Show_Task(){
    let resultHTML = '';
    
    for (let i = 0; i < allTasks.length; i++) {
        resultHTML += '<div id="task-box" class="task-box"><div class="task-board"><div>Task</div><div class="task-buttons"><button>Check</button><button>Delete</button></div></div>'
    }

    document.getElementById("task-box").innerHTML = resultHTML;
}

function Add_New_Task(){
    let taskContent = taskInput.value;
    allTasks.push(taskContent);
    Show_Task();
}