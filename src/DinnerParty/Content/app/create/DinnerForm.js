import React from 'react';
import { Field, reduxForm } from 'redux-form';

function DinnerForm({ error, handleSubmit, submitFailed }) {
  console.log('submitFailed', submitFailed, error)
  const errorDisplay = error ? <div>An error occurred</div> : null
  return (
    <form onSubmit={handleSubmit}>
      <div>
        <label htmlFor="title">Title</label>
        <Field name="title" component="input" type="text"/>
      </div>
      <div>
        <label htmlFor="eventDate">Event Date</label>
        <Field name="eventDate" component="input" type="text"/>
      </div>
      <div>
        <label htmlFor="description">Description</label>
        <Field name="description" component="input" type="text"/>
      </div>
      <div>
        <label htmlFor="hostName">Host Name</label>
        <Field name="hostName" component="input" type="text"/>
      </div>
      <div>
        <label htmlFor="contactPhone">Contact Phone</label>
        <Field name="contactPhone" component="input" type="text"/>
      </div>
      <div>
        <label htmlFor="address">Address, City, State, Zip</label>
        <Field name="address" component="input" type="text"/>
      </div>
      <div>
        <label htmlFor="country">Country</label>
        <Field name="country" component="input" type="text"/>
      </div>
      <button type="submit">Submit</button>
      {errorDisplay}
    </form>
  );
}

export default reduxForm({
  form: 'dinner'
})(DinnerForm);
