<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:pp="http://schemas.example.com/preprocessor"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    exclude-result-prefixes="pp">

	<!-- Parameter -->
	<xsl:param name="fix_issue_001" select="'False'" />

	<!-- Identity template -->
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()" />
		</xsl:copy>
	</xsl:template>

	<!-- Template to process pp:If elements -->
	<xsl:template match="pp:If">
		<xsl:variable name="condition" select="@Condition" />
		<xsl:choose>
			<xsl:when test="starts-with($condition, '!')">
				<xsl:variable name="constName" select="substring($condition, 2)" />
				<xsl:if test="not($fix_issue_001 = 'True') and $constName = 'fix_issue_001'">
					<xsl:apply-templates select="node()" />
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="$fix_issue_001 = 'True' and $condition = 'fix_issue_001'">
					<xsl:apply-templates select="node()" />
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>
