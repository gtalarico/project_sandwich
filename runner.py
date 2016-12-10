import os
import json
import shutil
from collections import OrderedDict

BASEDIR = os.environ['SANDWICH']
os.chdir(BASEDIR)
JOBS_FOLDER = os.path.join(BASEDIR, 'jobs')
JOB_PENDING_FOLDER = os.path.join(JOBS_FOLDER, 'pending')
JOB_DONE_FOLDER = os.path.join(JOBS_FOLDER, 'done')

import rpw
from rpw import doc

PARAMETERS = ["Name", "Family", "Family Type", "Level",
              "Number", "Length", "Area", "Elevation"]

# __window__.Close()


def save_job(job_filepath, job_data):
    with open(job_filepath, 'w') as fp:
        json.dump(job_data, fp, indent=2)


def get_job(job_filepath):
    with open(job_filepath, 'r') as fp:
        return json.load(fp)


def process_job(job_data):
    if job_data['action'] == 'get':
        category = job_data['filters']['category']
        istype = bool(job_data['filters']['istype'])
        isnottype = bool(job_data['filters']['isnottype'])
        instances = rpw.Collector(
                                #   of_class="FamilyInstance",
                                  of_category=category,
                                  is_type=istype,
                                  is_not_type=isnottype).elements

        reponse_elements = []
        for instance in instances:
            # instance = rpw.Instance(instance)
            parameters = {}

            for parameter in instance.Parameters:
                try:
                    value = parameter.AsValueString() or parameter.AsString()
                    parameters[parameter.Definition.Name] = value
                except:
                    pass

            print(parameters)
            instance_dict = OrderedDict()
            instance_dict['id'] = instance.Id.IntegerValue
            instance_dict['class'] = type(instance).__name__
            for param_name, param_value in parameters.iteritems():
                if param_name not in PARAMETERS:
                    continue
                clean_param_name = ''.join([c for c in param_name if c.isalnum()]).lower()
                instance_dict[clean_param_name] = param_value

            reponse_elements.append(instance_dict)

        job_data['response'] = reponse_elements
        job_data['status'] = 'done'
        return job_data


def parameters_dict(element):
    params = {}
    for param in element.Parameters:
        params[param.Definition.Name] = param.AsValueString()
    return params


# RUN JOB
for job_filename in os.listdir(JOB_PENDING_FOLDER):
    job_filepath = os.path.join(JOB_PENDING_FOLDER, job_filename)

    job_data = get_job(job_filepath)
    print('Processing: {}'.format(job_data['job_id']))
    print('filepath: {}'.format(job_data['filepath']))
    print('doc: {}'.format(doc.PathName))
    if job_data['filepath'] == doc.PathName:
        new_job_data = process_job(job_data)
        print(new_job_data)
        save_job(job_filepath, new_job_data)
        shutil.move(job_filepath, JOB_DONE_FOLDER)
