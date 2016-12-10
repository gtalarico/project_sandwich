var results_json;
var timer;

var stuffs = [{
    "band": "Weezer",
    "song": "El Scorcho"
}, {
    "band": "Chevelle",
    "song": "Family System"
}];



function makeGraph(d) {
  $("#table-headers").empty();
    makeHeaders(d);
    makeRows(d);
    // materializeGraph();
}

function makeHeaders(d) {
    if ($("#table-headers").children().length == 0) {

        for (key in d[0]) {
            console.log(key);
            $("#table-headers").append($('<th />', {
                text: key.split(' ').join('-')
            }))
        }
    }
}


function makeRows(d) {

  var dynatable =  $('#my-final-table').dynatable({
      features: {
    paginate: false,
    sort: true,
    pushState: false,
    search: true,
    recordCount: true,
    perPageSelect: true
},
        table: {
            headRowSelector: 'thead',
            defaultColumnIdStyle: 'lowercase'
        }
        // ,
        // dataset: {
        //     records: d
        // }
    }).data('dynatable');

    dynatable.records.updateFromJson({records: d});
                dynatable.records.init();
                dynatable.process();


}


function materializeGraph() {

        $("#dynatable-pagination-links-my-final-table").addClass("pagination");
        $("#dynatable-page-link").addClass("waves-effect");
        $("#dynatable-pagination-links-my-final-table > li.dynatable-active-page").addClass("active");
};


function getParameters(resp) {
    array = JSON.parse(resp);
    tableData = [];
    console.log(array)
    for (i = 0; i < array.length; i++) {
        tableData.add(resp[i].parameters);
    }
    console.log(tableData);
    return tableData
}

function checkStatus(job_id) {

    $.getJSON("getjob?job_id=" + job_id, function(json) {
        results_json = json;
        response = json["response"];
        var result_jsonstring = JSON.stringify(results_json);
        }).done(
        function() {
            if (results_json["status"] === "done") {
                console.log("task done!");
                console.log(response);
                toast("Order Ready");
                makeGraph(response);
                clearInterval(timer);
                toggle_visibility();
                $("#done-jobs .collection :first-child i").text("Done");
$("#done-jobs .collection :first-child").removeClass("active");


            } else if (results_json["status"] === "pending") {
                console.log(results_json["status"]);
            } else {
                console.log("grave error this is broken. reconsider life");
            }
        }
    )

}

$("#search>form").submit(function(e) {
    e.preventDefault() // Keeps page from submitting and reloading
    var category = $("#category_dropdown").val();
    var fileName = $("#filepath_dropdown").val();
    var istype = $("#istype").is(':checked') ? 1 : 0;
    var isnottype = $("#isnottype").is(':checked') ? 1 : 0;
    console.log("test" + category);
    // sessionStorage.removeItem('result_scroll');
    // sessionStorage.removeItem('tag_name');
    // $('#result-filter').html('')
    toggle_visibility();
    $.getJSON("makejob?category=" + category + "&filename=" + fileName +'&istype=' + istype + '&isnottype=' + isnottype, function(json) {
            console.log('Search Ajax call successful. ')
            results_json = json;
        })
        .done(function() {
            console.log("Search Finished");
            // Search successful
            $("#done-jobs .collection").prepend( '<a href="#" class="collection-item active" target="_blank"' + '>' +
                                                results_json['job_id'] + ':' + "<b>" + results_json['filters']['category'] + "</b>" +
                                                ': <i> Processing </i> </a>'


                                              )


            console.log(results_json["job_id"]);

            // statusCheck(results_json["job_id"]);
            timer = window.setInterval(function() {
                checkStatus(results_json["job_id"])
            }, 5000);
            toast("Order Sent");

        }).fail(function() {
            console.log('Search Ajax call FAILED.')
            var results = {
                'error': 'Could not reach search API'
            };
              toast("Order Failed");
            // buildResults(results, pattern);
        });
    //
    // var clean_uri = updateQueryStringParameter('filter', '');
    // history.pushState(null, null, clean_uri);
    //
    // search(pattern, null, true)
});

function toggle_visibility() {
    var e = document.getElementById('loading_bar');
    if (e.style.display == 'block' || e.style.display == '') {
        e.style.display = 'none';
    } else {
        e.style.display = 'block';
    }
}

function toast(text) {
    Materialize.toast(text, 3000);
}



// var myRecords = [
//     {
//       "band": "Weezer",
//       "song": "El Scorcho"
//     },
//     {
//       "band": "Chevelle",
//       "song": "Family System"
//     }
//   ];
//   console.log(JSON.parse(JSON.stringify(myRecords)));
//
// var  parsed = JSON.parse(JSON.stringify(myRecords));
// $('#my-final-table').dynatable({
//   dataset: {
//     records: parsed
//   }
// });
