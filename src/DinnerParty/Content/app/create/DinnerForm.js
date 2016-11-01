import React from 'react';
import { Field, reduxForm } from 'redux-form';

function DinnerForm({ handleSubmit }) {
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
        <label htmlFor="contactInfo">Contact Info</label>
        <Field name="contactInfo" component="input" type="text"/>
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
    </form>
  );
}

export default reduxForm({
  form: 'dinner'
})(DinnerForm);
