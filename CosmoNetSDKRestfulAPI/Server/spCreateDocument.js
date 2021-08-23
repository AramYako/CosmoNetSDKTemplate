function spCreateDocument(doc, enforceSchema) {

    if (doc.type != 'undefined') {
        doc.isValid = true;
    }
    else if (enforceSchema) {
        return new Error("Schema is enforced")
    }

    var context = getContext();
    var collection = context.getCollection();
    var accepted = collection.createDocument(
        collection.getSelfLink(),
        doc,
        function (err, newDoc) {
            if (err) throw new Error('Error' + err.message);
            context.getResponse().setBody(newDoc);
        }
    );
    if (!accepted) return;
}