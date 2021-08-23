function spHelloWorld(){
    var context = getContext(); 
    var response = context.getResponse();
    response.setBody('Greeting from the cosmo db server!');
}