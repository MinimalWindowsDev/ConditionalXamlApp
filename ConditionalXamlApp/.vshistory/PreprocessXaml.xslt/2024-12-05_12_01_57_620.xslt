<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:pp="http://schemas.example.com/preprocessor"
                exclude-result-prefixes="pp"
                version="1.0">

	<!-- Define parameters for your preprocessor constants -->
	<xsl:param name="fix_issue_001" select="'False'" />

	<!-- Identity template to copy all nodes by default -->
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
				<xsl:if test="not(${$constName} = 'True')">
					<xsl:apply-templates select="node()" />
				</xsl:if>
			</xsl:when>
			<xsl:otherwise>
				<xsl:if test="${$condition} = 'True'">
					<xsl:apply-templates select="node()" />
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>
