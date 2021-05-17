$(() => {
    let invalidFeedbacks = $('.invalid-feedback');
    let countEmpty = 0;
    invalidFeedbacks.each((index, element) => {
        if ($(element).text()) {
            countEmpty++;
        }
    });
    if (countEmpty !== 0) {
        invalidFeedbacks.each((index, element) => {
            let parent = $(element).parent();
            let formControl = $(parent.find('.form-control'));
            formControl.addClass($(element).text() ? 'is-invalid' : 'is-valid');
        });
    }
    
    let formControls = $('.form-control');
    formControls.each((index, element) => {
        $(element).change(function () {
            $(element).removeClass('is-invalid');
            $(element).removeClass('is-valid');
        });
    });
});
