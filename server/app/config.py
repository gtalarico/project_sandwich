import os
from app.logger import logger


class Config(object):
    DEBUG = False
    # TESTING = False
    # STAGING = False
    # PRODUCTION = False
    BASEDIR = os.path.abspath(os.path.dirname(__file__))
    TEMPLATEDIR = os.path.join(BASEDIR, 'templates')
    JSONIFY_PRETTYPRINT_REGULAR = False

class Development(Config):
    DEBUG = True
    SECRET_KEY = 'SuperSecretKey'

    # DEBUG_TB_INTERCEPT_REDIRECTS = False
    # DEBUG_TB_TEMPLATE_EDITOR_ENABLED = True
    # DEBUG_TB_PANELS = (
    #     'flask_debugtoolbar.panels.versions.VersionDebugPanel',
    #     'flask_debugtoolbar.panels.timer.TimerDebugPanel',
    #     'flask_debugtoolbar.panels.headers.HeaderDebugPanel',
    #     'flask_debugtoolbar.panels.request_vars.RequestVarsDebugPanel',
    #     'flask_debugtoolbar.panels.config_vars.ConfigVarsDebugPanel',
    #     'flask_debugtoolbar.panels.template.TemplateDebugPanel',
    #     'flask_debugtoolbar.panels.logger.LoggingPanel',
    #     'flask_debugtoolbar.panels.route_list.RouteListDebugPanel',
    #     'flask_debugtoolbar.panels.profiler.ProfilerDebugPanel',
        # 'flask_debugtoolbar.panels.sqlalchemy.SQLAlchemyDebugPanel',
    # )
