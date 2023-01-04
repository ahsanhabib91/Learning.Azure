function spBoundedExecution(docs) {

    var context = getContext();
    var collection = context.getCollection();
    var response = context.getResponse();

    // initialize count and start recursive call to loop through documents
    var count = 0;
    processDocuments(docs[0]);

    function processDocuments(doc) {

        // do some processing over the document

        // insert the document to the container
        var isAccepted = collection.createDocument(collection.getSelfLink(), doc,
            // callback function after createDocument completes
            function (err, doc) {
                if (err) throw err;
                count++;
                if (count == doc.length)
                    // done; return full count
                    response.setBody(count);
                else
                    // not done; recursive call for next document
                    processDocuments(docs[count]);
            }
        );

        // if wasn't accepted, we're out of time (bounded execution)
        if (!isAccepted) {
            // return partial count
            response.setBody(count);
        }
    }
}